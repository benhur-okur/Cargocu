using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct Shift // struct yapısı ile birden fazla vardiya biliglerini targetscore, timelimit gibi bunları bir yapı haklinde tutuyoruz. Serlaizible oldugu için inspecterda değişitrecez bunu istediğimiz bölüm değiştirme kurallarına göre
{
    public int targetScore;
    public float timeLimit;
    public float speedMultiplier;
    public int packageCount;
    public int obstacleCount; // YENİ: Bu levelda kaç engel olacağı bilgisini ekledik
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton pattern kurduk burda ki diğer scriptler GameManager ınstance şeklinde rahatlıkla ulaşacaklar

    [Header("Vardiya (Level) Ayarları")]
    public Shift[] shifts;
    private int currentShiftIndex = 0; // hangi levelda olduğumuzu tutan index buna göre dizayn edicez hangi bölümde oldugumuzu
    private bool isLevelCompleted = false; // levelın tamamlanıp tamamnlanmadıgını tutuyoruz bool olarak bu sayede level tamamlandıktan sonra oyun bitmediği halde menü açılmasını engelleyeceğiz

    float timeRemaining;
    bool gameStarted = false;
    bool gameOver = false;
    private bool isPaused = false;

    [Header("UI Panelleri")] // hiearcye koudugmuz panel objeleri
    [SerializeField] Text timerText;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip levelCompleteSound;
    [SerializeField] AudioClip gameOverSound;

    public Text currentLevelText; // güncel hangi bölümde oldugumuzu göstermek için koyduk

    [Header("Duraklatma ve Seviye UI")]
    public GameObject pausePanel;
    public GameObject levelsPanel;
    public Button[] levelButtons;

    [Header("Kargo üretim ayarları")]
    public GameObject packagePrefab;
    public Transform[] spawnPoints;

    [Header("Engel (Obstacle) Üretim Ayarları")] // YENİ: Engeller için gerekli referanslar
    public GameObject[] obstaclePrefabs;
    public Transform[] obstacleSpawnPoints;

    void Awake() // AI önerisiyle ekledik, singleton için gerekliymiş bu sayede oyun başladığında GameManager ınstance'ı oluşturuluyor ve diğer scriptler ona erişebiliyor
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // basic singleton kurulumu yaptık
    }

    void Start()
    {
        currentShiftIndex = PlayerPrefs.GetInt("SelectedLevel", 0); // burda kullanıcı eger daha önce oyunu yarıda bıraktıysa kaldıgı indexti okur ki ordaki bölümden devam etsin, yoksa eger 0 default

        if (shifts.Length > 0 && currentShiftIndex < shifts.Length)
            timeRemaining = shifts[currentShiftIndex].timeLimit; // olduğumuz levelın zaman limitini alıyoruz 
        else
            timeRemaining = 120f; // bu da ilk bölümde oldugumuz anlamnına gelio -> 120sn zaten ilk bölüm için default

        // level yazısı güncelleme UI
        if (currentLevelText != null)
        {
            currentLevelText.text = "Level " + (currentShiftIndex + 1);
        }

        UpdateLevelButtons();

        Time.timeScale = 0f;
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(false);
    }

    void Update()
    {
        // esc tuşununun çalışma mantıgı
        if (Input.GetKeyDown(KeyCode.Escape) && gameStarted && !gameOver && !isLevelCompleted)
        {
            TogglePause(); // esc tuşu time scale'ı 0 yapar ama update fonksiyonunu durdurmuyor bu sayede geri dönbebilecez oyuna ve input alabilciez tekrar pasue menusunden cıkmak için
        }

        if (!gameStarted || gameOver || isPaused || isLevelCompleted) return;

        timeRemaining -= Time.deltaTime;

        if (timerText != null) // kalan zamanı göstemek için dakika ve saniye formatında timerText objesine yazdırıyoruz
        {
            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining % 60);
            timerText.text = minutes + ":" + seconds.ToString("00");
        }

        if (timeRemaining <= 0) // zaman bitme durumu
        {
            gameOver = true;
            timeRemaining = 0;

            if (gameOverSound != null) //zaman bittiginde çalacak soundu ayarlama işi
            {
                AudioSource.PlayClipAtPoint(gameOverSound, Vector3.zero);
            }

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        if (startPanel != null) startPanel.SetActive(false);

        if (shifts.Length > 0 && currentShiftIndex < shifts.Length)
        {
            Driver driver = FindAnyObjectByType<Driver>(); //bu fonksiyon ile sahnede Driver scriptine sahip herhangi bir objeyi bulup ona erişiyoruz ki vardiya atlandığında hız değişimi yapabilelim
            if (driver != null) driver.SetSpeedMultiplier(shifts[currentShiftIndex].speedMultiplier); // driver.csdeki çarpanalara göre güncel leveldaki hızlara göre ayarlama işlemi

            SpawnPackages(); // artık pakaetler de spawn oluo oyun gerçekten baslıor
            SpawnObstacles(); // YENİ: Oyuna başlarken engelleri de spawn ediyoruz
        }
    }

    public void RestartGame() // restart butonu için fonksiyon
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckShiftProgress(int currentScore)
    {
        if (isLevelCompleted) return;

        if (currentScore >= shifts[currentShiftIndex].targetScore) // skor hedefine ulaştıysak level complete demek için
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        if (isLevelCompleted) return;

        isLevelCompleted = true;

        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }

        StartCoroutine(OpenLevelsMenuAfterSound());
    }

    IEnumerator OpenLevelsMenuAfterSound() // burda artk menu aclıması lazım cunku currentn lecel tamamlandı
    {
        yield return new WaitForSeconds(0.7f); // kısa bir bekleme süresi ekledik ki ses tam olarak çalabilsin yoksa kesilio ses.

        int nextLevel = currentShiftIndex + 1;
        int maxUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);

        if (nextLevel > maxUnlocked && nextLevel < shifts.Length) // buradaki prefs kontrolü ile kaydetmeyi AI'dan yardım aldık
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
        }

        UpdateLevelButtons();

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(true);
    }

    // --- MENÜ, SEVİYE VE KAYIT KONTROL FONKSİYONLARI ---
    private void UpdateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = (i <= unlockedLevel); // hangi levelların erişilebilir oldugunu kotnrol etmek için max unlocked levela kadar iterate ederken intereactable yapıyoruz
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null) pausePanel.SetActive(isPaused);

        if (!isPaused && levelsPanel != null) levelsPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OpenLevelsMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(true);
    }

    public void LoadSpecificLevel(int levelIndex) // kullanıcı istediği bölümü seçerse o bölümü yüklemek için fonksiyon
    {
        PlayerPrefs.SetInt("SelectedLevel", levelIndex); // diske de kaydediyoruz ki daha sonra bilgielr saklı kalsın kullanıcının
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // sıfırlama işlemi oyunu
    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 0);
        PlayerPrefs.SetInt("SelectedLevel", 0);
        UpdateLevelButtons(); //butonları kiltilemek iin
    }

    private void SpawnPackages()
    {
        if (packagePrefab == null || spawnPoints.Length == 0) return;

        int amountToSpawn = shifts[currentShiftIndex].packageCount; // kaç kargo wpawn olucak bilgisini alıyoruz
        if (amountToSpawn > spawnPoints.Length) amountToSpawn = spawnPoints.Length; // eğer ki lenghten uzun ise spawn noktası kadar spawn yaparaız hata önleme kotnroluy

        //arrray ile yapamadım cunku spawnlanan poinlteri kaldıramıyozu list iel çok kolay oluyor
        List<Transform> availablePoints = new List<Transform>(spawnPoints); // spwawn pooinltei listeye alıyouz spwan yaptıkca listeden kaldırcaz ki duplicate sıkıntısı yaşamayalım.

        for (int i = 0; i < amountToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Instantiate(packagePrefab, availablePoints[randomIndex].position, Quaternion.identity); // Instantiate ile package prefabını spawn noktalarından rastgele seçilen birine spawn ediyoruz
            availablePoints.RemoveAt(randomIndex); // listeden cıakrıoz 
        }
    }

    // YENİ: DİNAMİK ENGEL OLUŞTURMA SİSTEMİ
    private void SpawnObstacles()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0 || obstacleSpawnPoints.Length == 0) return;

        int amountToSpawn = shifts[currentShiftIndex].obstacleCount;
        if (amountToSpawn > obstacleSpawnPoints.Length) amountToSpawn = obstacleSpawnPoints.Length;

        List<Transform> availablePoints = new List<Transform>(obstacleSpawnPoints);

        for (int i = 0; i < amountToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            int randomObstacle = Random.Range(0, obstaclePrefabs.Length); // Statik mi hareketli mi olacak rastgele seçiyoruz

            Instantiate(obstaclePrefabs[randomObstacle], availablePoints[randomIndex].position, Quaternion.identity);
            availablePoints.RemoveAt(randomIndex);
        }
    }
}