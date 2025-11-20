using UnityEngine;
using System.Collections.Generic;

public class InfiniteGround : MonoBehaviour
{
    [Header("Ground Settings")]
    [SerializeField] private GameObject groundTilePrefab;
    [SerializeField] private int tilesAhead = 3;
    [SerializeField] private int tilesBehind = 1;
    [SerializeField] private float tileLength = 10f;
    
    private Transform playerTransform;
    private List<GameObject> activeTiles = new List<GameObject>();
    private int currentTileIndex = 0;
    
    private void Start()
    { 
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Create initial tiles
        CreateInitialTiles();
    }
    
    private void Update()
    {
        if (playerTransform == null) return;
        
        // Check if we need to spawn new tiles
        CheckAndSpawnTiles();
        
        // Remove tiles behind player
        RemoveOldTiles();
    }
    
    private void CreateInitialTiles()
    {
        if (groundTilePrefab == null) return;
        
        float startZ = playerTransform != null ? playerTransform.position.z : 0f;
        
        for (int i = -tilesBehind; i <= tilesAhead; i++)
        {
            Vector3 position = new Vector3(0f, 0f, startZ + (i * tileLength));
            GameObject tile = Instantiate(groundTilePrefab, position, Quaternion.identity, transform);
            activeTiles.Add(tile);
        }
    }
    
    private void CheckAndSpawnTiles()
    {
        if (groundTilePrefab == null || playerTransform == null) return;
        
        // Find the farthest tile ahead
        float farthestZ = float.MinValue;
        foreach (GameObject tile in activeTiles)
        {
            if (tile != null && tile.transform.position.z > farthestZ)
            {
                farthestZ = tile.transform.position.z;
            }
        }
        
        // Spawn new tiles if player is getting close
        float playerZ = playerTransform.position.z;
        float threshold = farthestZ - (tileLength * tilesAhead);
        
        if (playerZ > threshold)
        {
            Vector3 newPosition = new Vector3(0f, 0f, farthestZ + tileLength);
            GameObject newTile = Instantiate(groundTilePrefab, newPosition, Quaternion.identity, transform);
            activeTiles.Add(newTile);
        }
    }
    
    private void RemoveOldTiles()
    {
        if (playerTransform == null) return;
        
        float playerZ = playerTransform.position.z;
        
        for (int i = activeTiles.Count - 1; i >= 0; i--)
        {
            if (activeTiles[i] == null)
            {
                activeTiles.RemoveAt(i);
                continue;
            }
            
            float tileZ = activeTiles[i].transform.position.z;
            float distanceBehind = playerZ - tileZ;
            
            // Remove tiles that are too far behind
            if (distanceBehind > (tileLength * (tilesBehind + 1)))
            {
                Destroy(activeTiles[i]);
                activeTiles.RemoveAt(i);
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clean up all tiles
        foreach (GameObject tile in activeTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
    }
}

