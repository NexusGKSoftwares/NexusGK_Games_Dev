using UnityEngine;

public enum PowerUpType
{
    SpeedBoost,
    Shield,
    Magnet,
    DoubleCoins
}

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField] private PowerUpType powerUpType = PowerUpType.SpeedBoost;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmplitude = 0.3f;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;
    
    private Vector3 startPosition;
    private float floatTimer = 0f;
    private bool isCollected = false;
    
    private void Start()
    {
        startPosition = transform.position;
        
        // Setup trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
        
        if (col == null)
        {
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = 0.8f;
        }
    }
    
    private void Update()
    {
        if (isCollected) return;
        
        // Rotate power-up
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Float animation
        floatTimer += Time.deltaTime * floatSpeed;
        float newY = startPosition.y + Mathf.Sin(floatTimer) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Move backward with game speed
        if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
        {
            transform.position -= Vector3.forward * GameManager.Instance.CurrentSpeed * Time.deltaTime;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            CollectPowerUp();
        }
    }
    
    private void CollectPowerUp()
    {
        if (isCollected) return;
        isCollected = true;
        
        // Apply power-up effect
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            ApplyPowerUp(player);
        }
        
        // Play particle effect
        if (collectParticles != null)
        {
            ParticleSystem particles = Instantiate(collectParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
        }
        
        // Play sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // Destroy power-up
        Destroy(gameObject);
    }
    
    private void ApplyPowerUp(PlayerController player)
    {
        PowerUpManager powerUpManager = FindObjectOfType<PowerUpManager>();
        if (powerUpManager == null)
        {
            GameObject managerObj = new GameObject("PowerUpManager");
            powerUpManager = managerObj.AddComponent<PowerUpManager>();
        }
        
        powerUpManager.ActivatePowerUp(powerUpType, duration);
    }
}

