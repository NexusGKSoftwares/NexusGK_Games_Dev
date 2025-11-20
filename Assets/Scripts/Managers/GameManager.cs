using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float speedIncreaseRate = 0.1f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float scoreMultiplier = 1f;
    [SerializeField] private float coinScoreValue = 10f;
    
    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private float obstacleSpawnRateIncrease = 0.05f;
    [SerializeField] private float maxObstacleSpawnRate = 2f;
    
    // Game state
    public bool IsGameActive { get; private set; }
    public float CurrentSpeed { get; set; }
    
    // Score
    private float currentScore = 0f;
    private int coinsCollected = 0;
    private float distanceTraveled = 0f;
    private int highScore = 0;
    
    // Events
    public event Action<float> OnScoreChanged;
    public event Action<int> OnCoinsChanged;
    public event Action OnGameOver;
    public event Action OnGameStart;
    
    private float gameStartTime;
    private float lastDifficultyIncreaseTime;
    private Transform playerTransform;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    private void Start()
    {
        StartGame();
    }
    
    private void Update()
    {
        if (IsGameActive)
        {
            UpdateScore();
            UpdateSpeed();
            UpdateDifficulty();
        }
    }
    
    public void StartGame()
    {
        IsGameActive = true;
        CurrentSpeed = baseSpeed;
        currentScore = 0f;
        coinsCollected = 0;
        distanceTraveled = 0f;
        gameStartTime = Time.time;
        lastDifficultyIncreaseTime = Time.time;
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        OnGameStart?.Invoke();
        OnScoreChanged?.Invoke(currentScore);
        OnCoinsChanged?.Invoke(coinsCollected);
    }
    
    private void UpdateScore()
    {
        if (playerTransform != null)
        {
            // Score based on distance traveled
            distanceTraveled += CurrentSpeed * Time.deltaTime;
            currentScore = distanceTraveled * scoreMultiplier + (coinsCollected * coinScoreValue);
            
            OnScoreChanged?.Invoke(currentScore);
        }
    }
    
    private void UpdateSpeed()
    {
        // Gradually increase speed
        if (CurrentSpeed < maxSpeed)
        {
            CurrentSpeed += speedIncreaseRate * Time.deltaTime;
            CurrentSpeed = Mathf.Min(CurrentSpeed, maxSpeed);
        }
    }
    
    private void UpdateDifficulty()
    {
        // Increase difficulty over time
        if (Time.time - lastDifficultyIncreaseTime >= difficultyIncreaseInterval)
        {
            lastDifficultyIncreaseTime = Time.time;
            IncreaseDifficulty();
        }
    }
    
    private void IncreaseDifficulty()
    {
        // Notify obstacle spawner to increase spawn rate
        ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
        if (spawner != null)
        {
            spawner.IncreaseSpawnRate(obstacleSpawnRateIncrease);
        }
    }
    
    public void AddCoin(int amount = 1)
    {
        coinsCollected += amount;
        OnCoinsChanged?.Invoke(coinsCollected);
    }
    
    public void GameOver()
    {
        if (!IsGameActive) return;
        
        IsGameActive = false;
        
        // Update high score
        int finalScore = Mathf.RoundToInt(currentScore);
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        
        OnGameOver?.Invoke();
        
        // Optionally reload scene after delay
        // Invoke(nameof(ReloadScene), 3f);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void ReloadScene()
    {
        RestartGame();
    }
    
    public int GetCurrentScore()
    {
        return Mathf.RoundToInt(currentScore);
    }
    
    public int GetHighScore()
    {
        return highScore;
    }
    
    public int GetCoinsCollected()
    {
        return coinsCollected;
    }
}

