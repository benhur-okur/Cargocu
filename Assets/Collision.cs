using UnityEngine;
using UnityEngine.UI;

public class Collision : MonoBehaviour
{
    bool hasPacked;
    bool hasDelivered;

    [SerializeField] Color32 hasPackageColor = new Color32(255, 200, 0, 255);
    [SerializeField] Color32 noPackageColor = new Color32(255, 255, 255, 255);

    SpriteRenderer spriteRenderer;
    [SerializeField] Text packageText;
    [SerializeField] DeliveryManager deliveryManager;
    [SerializeField] ScoreManager scoreManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (packageText != null)
            packageText.text = "Kargo Yok";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Package" && !hasPacked)
        {
            hasPacked = true;
            hasDelivered = false;
            spriteRenderer.color = hasPackageColor;
            Destroy(other.gameObject);
            if (packageText != null)
                packageText.text = "Kargo Var!";

            // kargo alýnýnca delivery point aç
            if (deliveryManager != null)
                deliveryManager.SelectNewDelivery();
        }
        else if (other.tag == "Customer" && hasPacked && !hasDelivered)
        {
            hasPacked = false;
            hasDelivered = true;
            spriteRenderer.color = noPackageColor;
            if (packageText != null)
                packageText.text = "Kargo Yok";

            if (scoreManager != null)
                scoreManager.AddScore(10);

            // teslim edince delivery point kapat
            if (deliveryManager != null)
                deliveryManager.HideDelivery();
        }
    }
}