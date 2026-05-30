using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    int score = 0;
    [SerializeField] Text scoreText;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        // YENÝ: Puan deðiþtiðinde GameManager'a haber ver ve vardiya durumunu kontrol et
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckShiftProgress(score);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Skor: " + score;
    }
}