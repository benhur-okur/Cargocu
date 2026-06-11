using UnityEngine;

public class CargoItem : MonoBehaviour
{
    public enum WeightType { Hafif, Orta, Agir }
    public WeightType cargoWeight;

    public float speedPenalty = 1f; // 1 = yavaþlama yok, 0.5 = %50 yavaþ
    public int scoreReward = 10;

    void Start()
    {
        // 0 ile 100 arasýnda rastgele bir sayý çek (Yüzdelik zar atýyoruz)
        int randomChance = Random.Range(0, 100);

        if (randomChance < 60)
        {
            // %60 Ýhtimalle Hafif Kargo (0-59 arasý sayýlar)
            cargoWeight = WeightType.Hafif;
        }
        else if (randomChance < 90)
        {
            // %30 Ýhtimalle Orta Kargo (60-89 arasý sayýlar)
            cargoWeight = WeightType.Orta;
        }
        else
        {
            // Kalan %10 Ýhtimalle Aðýr Kargo (90-99 arasý sayýlar)
            cargoWeight = WeightType.Agir;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Aðýrlýða göre puan, yavaþlatma oraný ve görsel (renk/boyut) ayarla
        switch (cargoWeight)
        {
            case WeightType.Hafif:
                speedPenalty = 1.0f;  // Yavaþlatmaz
                scoreReward = 10;     // Standart puan
                if (sr != null) sr.color = Color.green; // Hafif kargo Yeþil
                transform.localScale = new Vector3(0.9f, 0.9f, 1f); // Küçük
                break;

            case WeightType.Orta:
                speedPenalty = 0.75f; // %25 Yavaþlatýr
                scoreReward = 20;     // Orta puan
                if (sr != null) sr.color = Color.blue; // Orta kargo Sarý
                transform.localScale = new Vector3(1.3f, 1.3f, 1f); // Normal
                break;

            case WeightType.Agir:
                speedPenalty = 0.55f;  // %45 Yavaþlatýr
                scoreReward = 35;     // Yüksek puan
                if (sr != null) sr.color = Color.red; // Aðýr kargo Kýrmýzý
                transform.localScale = new Vector3(1.6f, 1.6f, 1f); // Büyük
                break;
        }
    }
}