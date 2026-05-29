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

    [SerializeField] AudioClip pickupSound;
    [SerializeField] AudioClip deliverySound;
    [SerializeField] GameObject deliveryEffectPrefab;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (packageText != null)
        {
            packageText.text = "Kargo Yok";
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Package") && !hasPacked)
        {
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            hasPacked = true;
            hasDelivered = false;

            spriteRenderer.color = hasPackageColor;
            Destroy(other.gameObject);

            if (packageText != null)
            {
                packageText.text = "Kargo Var!";
            }

            if (deliveryManager != null)
            {
                deliveryManager.SelectNewDelivery();
            }
        }
        else if (other.CompareTag("Customer") && hasPacked && !hasDelivered)
        {
            Debug.Log("TESLIMAT CALISTI");
            hasPacked = false;
            hasDelivered = true;

            if (deliverySound != null)
            {
                AudioSource.PlayClipAtPoint(deliverySound, transform.position);
            }

            if (deliveryEffectPrefab != null)
            {
                Instantiate(deliveryEffectPrefab, other.transform.position, Quaternion.identity);
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