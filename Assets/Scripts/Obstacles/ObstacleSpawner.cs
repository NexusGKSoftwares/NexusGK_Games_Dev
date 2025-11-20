using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private float spawnDistance = 50f;
    [SerializeField] private float despawnDistance = 10f;
    [SerializeField] private float minSpawnInterval = 2f;
    [SerializeField] private float maxSpawnInterval = 5f;
    
    [Header("Lane Settings")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private int numberOfLanes = 3;
    
    [Header("Obstacle Types")]
    [SerializeField] private bool spawnGroundObstacles = true;
    [SerializeField] private bool spawnFlyingObstacles = false;
    [SerializeField] private float flyingObstacleHeight = 3f;
    
    private float currentSpawnInterval;
    private float spawnTimer = 0f;
    private float baseSpawnInterval;
    private Transform playerTransform;
    private List<GameObject> activeObstacles = new List<GameObject>();
    
    private void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        baseSpawnInterval = currentSpawnInterval;
    }
    
    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive) return;
        if (playerTransform == null) return;
        
        // Spawn obstacles
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
        // Despawn obstacles behind player
        CleanupObstacles();
    }
    
    private void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        
        // Randomly select obstacle type
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        
        // Random lane
        int lane = Random.Range(0, numberOfLanes);
        float xPosition = (lane - 1) * laneDistance;
        
        // Spawn position
        float zPosition = playerTransform.position.z + spawnDistance;
        Vector3 spawnPosition = new Vector3(xPosition, 0f, zPosition);
        
        // Check if it's a flying obstacle
        if (spawnFlyingObstacles && Random.value > 0.7f)
        {
            spawnPosition.y = flyingObstacleHeight;
        }
        else if (spawnGroundObstacles)
        {
            // Raycast to ground
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(xPosition, 10f, zPosition), Vector3.down, out hit, 20f))
            {
                spawnPosition.y = hit.point.y;
            }
        }
        
        // Instantiate obstacle
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.tag = "Obstacle";
        
        // Add to active list
        activeObstacles.Add(obstacle);
        
        // Setup obstacle movement if it doesn't have it
        ObstacleMovement obstacleMovement = obstacle.GetComponent<ObstacleMovement>();
        if (obstacleMovement == null)
        {
            obstacleMovement = obstacle.AddComponent<ObstacleMovement>();
        }
    }
    
    private void CleanupObstacles()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            // Remove obstacles behind player
            float distance = activeObstacles[i].transform.position.z - playerTransform.position.z;
            if (distance < -despawnDistance)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }
    
    public void IncreaseSpawnRate(float increase)
    {
        // Decrease spawn interval (spawn more frequently)
        minSpawnInterval = Mathf.Max(0.5f, minSpawnInterval - increase);
        maxSpawnInterval = Mathf.Max(1f, maxSpawnInterval - increase);
    }
    
    private void OnDestroy()
    {
        // Clean up all obstacles
        foreach (GameObject obstacle in activeObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
    }
}

