using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 2f;      // Engelin gidiþ-dönüþ hýzý
    public float moveDistance = 2f;   // Merkezden ne kadar uzaða gideceði
    public bool moveHorizontal = true; // Tikli ise sað-sol, Tiksiz ise yukarý-aþaðý hareket eder

    private Vector3 startPos;

    void Start()
    {
        // Engelin doðduðu ilk konumu hafýzaya alýyoruz
        startPos = transform.position;
    }

    void Update()
    {
        // PingPong fonksiyonu sayesinde engel bir o yana bir bu yana otomatik gider gelir
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;

        if (moveHorizontal)
        {
            transform.position = startPos + new Vector3(offset, 0, 0);
        }
        else
        {
            transform.position = startPos + new Vector3(0, offset, 0);
        }
    }
}