using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private bool useGameManagerSpeed = true;
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool rotateObstacle = false;
    [SerializeField] private Vector3 rotationSpeed = Vector3.zero;
    
    private Transform playerTransform;
    
    private void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    private void Update()
    {
        if (!useGameManagerSpeed || GameManager.Instance == null || !GameManager.Instance.IsGameActive)
        {
            return;
        }
        
        // Move obstacle backward (toward player)
        float currentSpeed = useGameManagerSpeed ? GameManager.Instance.CurrentSpeed : speed;
        transform.position -= Vector3.forward * currentSpeed * Time.deltaTime;
        
        // Rotate obstacle if enabled
        if (rotateObstacle)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Game over handled in PlayerController
            // Optionally destroy obstacle
            // Destroy(gameObject);
        }
    }
}

