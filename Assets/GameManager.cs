using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float gameTime = 120f; // 2 dakika
    float timeRemaining;
    bool gameStarted = false;
    bool gameOver = false;

    [SerializeField] Text timerText;        // süre yazýsý
    [SerializeField] GameObject startPanel; // giriþ ekraný
    [SerializeField] GameObject gameOverPanel; // oyun bitti ekraný

    void Start()
    {
        timeRemaining = gameTime;
        Time.timeScale = 0f; // oyunu durdur, menüdeyiz
        if (startPanel != null) startPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted || gameOver) return;

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
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}