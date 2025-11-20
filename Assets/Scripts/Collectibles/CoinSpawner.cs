using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnDistance = 50f;
    [SerializeField] private float despawnDistance = 10f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    
    [Header("Lane Settings")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private int numberOfLanes = 3;
    
    [Header("Coin Patterns")]
    [SerializeField] private bool spawnInRows = true;
    [SerializeField] private int coinsPerRow = 3;
    [SerializeField] private float rowSpacing = 2f;
    
    private float spawnTimer = 0f;
    private float currentSpawnInterval;
    private Transform playerTransform;
    private List<GameObject> activeCoins = new List<GameObject>();
    
    private void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
    
    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;
        if (playerTransform == null || coinPrefab == null) return;
        
        // Spawn coins
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnCoins();
            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
        // Cleanup coins
        CleanupCoins();
    }
    
    private void SpawnCoins()
    {
        if (spawnInRows)
        {
            // Spawn a row of coins across lanes
            SpawnCoinRow();
        }
        else
        {
            // Spawn single coin in random lane
            SpawnSingleCoin();
        }
    }
    
    private void SpawnCoinRow()
    {
        float zPosition = playerTransform.position.z + spawnDistance;
        
        // Determine which lanes to spawn coins in
        int lanesToUse = Mathf.Min(coinsPerRow, numberOfLanes);
        int startLane = Random.Range(0, numberOfLanes - lanesToUse + 1);
        
        for (int i = 0; i < lanesToUse; i++)
        {
            int lane = startLane + i;
            float xPosition = (lane - 1) * laneDistance;
            
            Vector3 spawnPosition = new Vector3(xPosition, 1f, zPosition);
            
            // Raycast to ground
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(xPosition, 10f, zPosition), Vector3.down, out hit, 20f))
            {
                spawnPosition.y = hit.point.y + 1f;
            }
            
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            coin.tag = "Coin";
            
            // Setup coin if needed
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript == null)
            {
                coinScript = coin.AddComponent<Coin>();
            }
            
            activeCoins.Add(coin);
        }
    }
    
    private void SpawnSingleCoin()
    {
        int lane = Random.Range(0, numberOfLanes);
        float xPosition = (lane - 1) * laneDistance;
        float zPosition = playerTransform.position.z + spawnDistance;
        
        Vector3 spawnPosition = new Vector3(xPosition, 1f, zPosition);
        
        // Raycast to ground
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(xPosition, 10f, zPosition), Vector3.down, out hit, 20f))
        {
            spawnPosition.y = hit.point.y + 1f;
        }
        
        GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        coin.tag = "Coin";
        
        Coin coinScript = coin.GetComponent<Coin>();
        if (coinScript == null)
        {
            coinScript = coin.AddComponent<Coin>();
        }
        
        activeCoins.Add(coin);
    }
    
    private void CleanupCoins()
    {
        for (int i = activeCoins.Count - 1; i >= 0; i--)
        {
            if (activeCoins[i] == null)
            {
                activeCoins.RemoveAt(i);
                continue;
            }
            
            // Remove coins behind player
            float distance = activeCoins[i].transform.position.z - playerTransform.position.z;
            if (distance < -despawnDistance)
            {
                Destroy(activeCoins[i]);
                activeCoins.RemoveAt(i);
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clean up all coins
        foreach (GameObject coin in activeCoins)
        {
            if (coin != null)
            {
                Destroy(coin);
            }
        }
    }
}

