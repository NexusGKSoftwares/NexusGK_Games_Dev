using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatAmplitude = 0.3f;
    
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;
    
    private Vector3 startPosition;
    private float floatTimer = 0f;
    private bool isCollected = false;
    
    private void Start()
    {
        startPosition = transform.position;
        
        // Setup trigger collider if needed
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
        
        // Add trigger collider if none exists
        if (col == null)
        {
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = 0.5f;
        }
    }
    
    private void Update()
    {
        if (isCollected) return;
        
        // Rotate coin
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
            CollectCoin();
        }
    }
    
    private void CollectCoin()
    {
        if (isCollected) return;
        isCollected = true;
        
        // Add coin to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoin(coinValue);
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
        
        // Destroy coin
        Destroy(gameObject);
    }
}

