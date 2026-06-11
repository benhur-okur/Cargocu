using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    bool hasPacked;
    bool hasDelivered;
    // bu iki deðiþkelne paket teslimi ve üzeirmizde paketin olup olmadýgný tutuyoruz

    [SerializeField] Color32 hasPackageColor = new Color32(255, 200, 0, 255);
    [SerializeField] Color32 noPackageColor = new Color32(255, 255, 255, 255); // araba kargo alrýsa rengi deðiþiyo serializer ile inspector da atadýk

    SpriteRenderer spriteRenderer;

    [SerializeField] Text packageText;
    [SerializeField] DeliveryManager deliveryManager;
    [SerializeField] ScoreManager scoreManager; // buralarý inspecterdan gerekli objeleri sürükleyerk doldurduk aþaðýdaki kodlara bu objeleri kullanýyoruz çünkü

    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip deliverySound;
    [SerializeField] GameObject deliveryEffectPrefab; // yine yukardaki mantýkla karg alma vee teslimlerde seslerin gelmesi için  var. ve +10 efekti

    // --- YENÝ EKLENEN DEÐÝÞKENLER ---
    Driver driver; // Arabanýn hýzýný deðiþtirmek için Driver scriptine eriþmemiz lazým
    int currentCargoReward = 10; // Teslimatta kaç puan vereceðimizi hafýzada tutacaðýmýz deðiþken

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Arabanýn üzerindeki Driver scriptini kod ile otomatik buluyoruz
        driver = GetComponent<Driver>();

        if (packageText != null)
        {
            packageText.text = "Kargo Yok"; // error fallback için ekeldik sonradan
        }
    }

    void OnTriggerEnter2D(Collider2D other) // unityninn kendi fonksiyonuymus arababnýn triger alanýna baþka (other) onje girerese tetikleme yapmak için kullanýyoruz
    {
        if (other.CompareTag("Package") && !hasPacked) // çok basit bir yeni kargo almak için 2 tane þeyi kontrol ediyoruz. Compare tag fonk unityde var objelerin tagini kontrol ediyoruz
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position); // bu kýsýmda external help alýndý playClipAtPoint interntten baktýk ve bu fonkssiyon ile pickupSoundu çalýyoruz ve sesin çýkacaðý pozisyonu da arabanýn pozisyonu yapýyoruz böylece araba hareket ederken ses de onunla birlikte hareket ediyor gibi oluyor
            }

            hasPacked = true; // kargo alýndý
            hasDelivered = false; // daha teslim yok

            spriteRenderer.color = hasPackageColor; // araba sarýya boyanýr kargo alýndý cunku

            // --- YENÝ: AÐIRLIK SÝSTEMÝ ENTEGRASYONU ---
            CargoItem item = other.GetComponent<CargoItem>(); // Çarptýðýmýz objenin üzerindeki CargoItem scriptini okuyoruz
            if (item != null)
            {
                // Kargodan gelen skoru hafýzaya alýyoruz (teslim edince vereceðiz)
                currentCargoReward = item.scoreReward;

                // Arabayý kargonun aðýrlýðýna göre yavaþlatýyoruz (Driver scriptine ceza yolluyoruz)
                if (driver != null) driver.ApplyCargoWeight(item.speedPenalty);

                // UI'da oyuncuya kargonun aðýrlýðýný da gösteriyoruz
                if (packageText != null)
                {
                    packageText.text = "Kargo: " + item.cargoWeight.ToString();
                }
            }
            else
            {
                // Eðer sahnede eski scripti olmayan bir paket kalmýþsa hata vermesin diye düz yazý
                if (packageText != null) packageText.text = "Kargo Var!";
            }

            Destroy(other.gameObject); // ve etkileþimegire other objesini ekrandan siliyoruz

            if (deliveryManager != null)
            {
                deliveryManager.SelectNewDelivery(); // modüler olmasý için deliveryManager objesi üzerinden yeni bir teslimat seçmesini söylüyoruz.
            }
        }
        else if (other.CompareTag("Customer") && hasPacked && !hasDelivered) // elde kargo var, teslim edilmedi ve giren obje müþteri ise teslim yapmayý if else ile bagladýk
        {
            Debug.Log("TESLIMAT CALISTI"); // teslim yapýldýgý için variablelar ve ses objeleri vs ýfýrlanýyor -> base degerlere geri döndürmemiz lazým  oyunun devamý için çünku
            hasPacked = false;
            hasDelivered = true;

            // --- YENÝ: YÜKÜ KALDIRMA ---
            // Kargo teslim edildiði için arabanýn yükünü kaldýrýp hýzýný normale döndürüyoruz
            if (driver != null) driver.RemoveCargoWeight();

            if (deliverySound != null)
            {
                AudioSource.PlayClipAtPoint(deliverySound, transform.position);
            }

            if (deliveryEffectPrefab != null)
            {
                // Instantiate fonksiyonu bize ürettiði objeyi geri döndürür, biz de onu 'effect' adýnda bir deðiþkene atarýz
                GameObject effect = Instantiate(deliveryEffectPrefab, other.transform.position, Quaternion.identity);

                // Ürettiðimiz bu objenin altýndaki Text bileþenini kodla buluyoruz
                Text effectText = effect.GetComponentInChildren<Text>();
                if (effectText != null)
                {
                    // Yazýyý, o anki kargonun gerçek puaný neyse onunla güncelliyoruz
                    effectText.text = "+" + currentCargoReward.ToString();
                }
            }

            spriteRenderer.color = noPackageColor;

            if (packageText != null)
            {
                packageText.text = "Kargo Yok";
            }

            if (scoreManager != null)
            {
                // --- YENÝ: DÝNAMÝK SKOR ---
                // Sabit 10 puan yerine, hafýzaya aldýðýmýz kargonun kendi aðýrlýk puanýný veriyoruz
                scoreManager.AddScore(currentCargoReward);
            }

            if (deliveryManager != null)
            {
                deliveryManager.HideDelivery();
            }
        }
    }
}