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
    [SerializeField] GameObject deliveryEffectPrefab; // yine yukardaki mantýkla karg alma vee teslimlerde seslerin gelmesi için  var. ve +10 efektini ekledik

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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
            Destroy(other.gameObject); // ve etkileþimegire other objesini ekrandan siliyoruz

            if (packageText != null)
            {
                packageText.text = "Kargo Var!";
            }

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

            if (deliverySound != null)
            {
                AudioSource.PlayClipAtPoint(deliverySound, transform.position);
            }

            if (deliveryEffectPrefab != null)
            {
                Instantiate(deliveryEffectPrefab, other.transform.position, Quaternion.identity); // Sonrada external help ile Instantiate ile müþterinin oldugu poziyonda yani en son teslimat yaptýgýmýz yerde +10 efekti çýkmasýný salgadýk bu sayede
            }

            spriteRenderer.color = noPackageColor;

            if (packageText != null)
            {
                packageText.text = "Kargo Yok";
            }

            if (scoreManager != null)
            {
                scoreManager.AddScore(10);
            }

            if (deliveryManager != null)
            {
                deliveryManager.HideDelivery();
            }
        }
    }
}