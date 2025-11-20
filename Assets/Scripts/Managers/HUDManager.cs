using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    private void OnEnable()
    {
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnCoinsChanged += UpdateCoins;
            GameManager.Instance.OnGameOver += ShowGameOver;
            GameManager.Instance.OnGameStart += HideGameOver;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from game events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnCoinsChanged -= UpdateCoins;
            GameManager.Instance.OnGameOver -= ShowGameOver;
            GameManager.Instance.OnGameStart -= HideGameOver;
        }
    }
    
    private void Start()
    {
        // Setup UI
        UpdateHighScore();
        HideGameOver();
        
        // Setup buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    
    private void UpdateScore(float score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.RoundToInt(score).ToString();
        }
    }
    
    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coins.ToString();
        }
    }
    
    private void UpdateHighScore()
    {
        if (highScoreText != null && GameManager.Instance != null)
        {
            highScoreText.text = "High Score: " + GameManager.Instance.GetHighScore().ToString();
        }
    }
    
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = "Final Score: " + GameManager.Instance.GetCurrentScore().ToString();
        }
        
        UpdateHighScore();
    }
    
    private void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    private void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    private void QuitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

