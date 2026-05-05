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
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Skor: " + score;
    }
}