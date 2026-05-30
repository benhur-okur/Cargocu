using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public struct Shift
{
    public int targetScore;
    public float timeLimit;
    public float speedMultiplier;
    public int packageCount;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Vardiya (Level) Ayarlarý")]
    public Shift[] shifts;
    private int currentShiftIndex = 0;
    private bool isLevelCompleted = false; // YENÝ: Levelin defalarca bitmesini engellemek için

    float timeRemaining;
    bool gameStarted = false;
    bool gameOver = false;
    private bool isPaused = false;

    [Header("UI Panelleri")]
    [SerializeField] Text timerText;
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject gameOverPanel;
    public Text currentLevelText; // YENÝ: Sol altta yazacak level yazýsý

    [Header("Duraklatma ve Seviye UI")]
    public GameObject pausePanel;
    public GameObject levelsPanel;
    public Button[] levelButtons;

    [Header("Kargo Üretim (Spawn) Ayarlarý")]
    public GameObject packagePrefab;
    public Transform[] spawnPoints;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentShiftIndex = PlayerPrefs.GetInt("SelectedLevel", 0);

        if (shifts.Length > 0 && currentShiftIndex < shifts.Length)
            timeRemaining = shifts[currentShiftIndex].timeLimit;
        else
            timeRemaining = 120f;

        // Sol alt level yazýsýný güncelle
        if (currentLevelText != null)
        {
            currentLevelText.text = "Level " + (currentShiftIndex + 1);
        }

        UpdateLevelButtons(); // Butonlarýn kilidini ayarlayan yardýmcý fonksiyon

        Time.timeScale = 0f;
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(false);
    }

    void Update()
    {
        // ESC'ye basýldýðýnda, oyun oynanýyorsa ve level bitmemiþse menüyü aç/kapat
        if (Input.GetKeyDown(KeyCode.Escape) && gameStarted && !gameOver && !isLevelCompleted)
        {
            TogglePause();
        }

        if (!gameStarted || gameOver || isPaused || isLevelCompleted) return;

        timeRemaining -= Time.deltaTime;

        if (timerText != null)
        {
            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining % 60);
            timerText.text = minutes + ":" + seconds.ToString("00");
        }

        if (timeRemaining <= 0)
        {
            gameOver = true;
            timeRemaining = 0;
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
            Driver driver = FindAnyObjectByType<Driver>();
            if (driver != null) driver.SetSpeedMultiplier(shifts[currentShiftIndex].speedMultiplier);

            SpawnPackages();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckShiftProgress(int currentScore)
    {
        // Eðer level zaten bittiyse (menü açýldýysa) skoru tekrar kontrol etme
        if (isLevelCompleted) return;

        if (currentScore >= shifts[currentShiftIndex].targetScore)
        {
            LevelCompleted();
        }
    }

    // --- YENÝ: LEVEL BÝTÝÞ KONTROLÜ ---
    private void LevelCompleted()
    {
        isLevelCompleted = true;

        // Bir sonraki level'ýn kilidini aç ve kaydet
        int nextLevel = currentShiftIndex + 1;
        int maxUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);

        if (nextLevel > maxUnlocked && nextLevel < shifts.Length)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
        }

        UpdateLevelButtons(); // Yeni açýlan levelýn butonunu anýnda aktif et

        // Oyunu durdur ve Bölümler (Levels) menüsünü aç
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelsPanel != null) levelsPanel.SetActive(true);
    }

    // --- MENÜ, SEVÝYE VE KAYIT KONTROL FONKSÝYONLARI ---
    private void UpdateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = (i <= unlockedLevel);
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

    public void LoadSpecificLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // YENÝ: Ýlerlemeyi sýfýrlama butonu için fonksiyon
    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 0);
        PlayerPrefs.SetInt("SelectedLevel", 0);
        UpdateLevelButtons(); // Butonlarý anýnda kilitle
    }

    // --- DÝNAMÝK KARGO OLUÞTURMA SÝSTEMÝ ---
    private void SpawnPackages()
    {
        if (packagePrefab == null || spawnPoints.Length == 0) return;

        int amountToSpawn = shifts[currentShiftIndex].packageCount;
        if (amountToSpawn > spawnPoints.Length) amountToSpawn = spawnPoints.Length;

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < amountToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availablePoints.Count);
            Instantiate(packagePrefab, availablePoints[randomIndex].position, Quaternion.identity);
            availablePoints.RemoveAt(randomIndex);
        }
    }
}