using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float laneChangeSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float slideHeight = 0.5f;
    
    [Header("Lane Settings")]
    [SerializeField] private float laneDistance = 3f;
    [SerializeField] private int numberOfLanes = 3;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem slideParticles;
    
    private Rigidbody rb;
    private int currentLane = 1; // 0 = left, 1 = middle, 2 = right
    private Vector3 targetPosition;
    private bool isSliding = false;
    private bool isGrounded = true;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private CapsuleCollider capsuleCollider;
    private Vector3 originalScale;
    private float slideTimer = 0f;
    
    // Animation hash IDs
    private int jumpHash = Animator.StringToHash("Jump");
    private int slideHash = Animator.StringToHash("Slide");
    private int isGroundedHash = Animator.StringToHash("IsGrounded");
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;
        }
        
        originalScale = transform.localScale;
        
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }
    
    private void Start()
    {
        targetPosition = transform.position;
        // Lock rotation and freeze X rotation for stability
        rb.freezeRotation = true;
    }
    
    private void Update()
    {
        HandleInput();
        CheckGrounded();
        UpdateSliding();
        
        // Update animation parameters
        if (animator != null)
        {
            animator.SetBool(isGroundedHash, isGrounded);
        }
    }
    
    private void FixedUpdate()
    {
        // Continuous forward movement
        MoveForward();
        ChangeLane();
    }
    
    private void HandleInput()
    {
        // Lane movement
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        
        // Jump
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded && !isSliding)
        {
            Jump();
        }
        
        // Slide
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && isGrounded && !isSliding)
        {
            StartSlide();
        }
    }
    
    private void MoveForward()
    {
        // Forward movement is handled by moving the world, but we can also move the player
        // In an endless runner, typically the world moves around the player
        float speed = GameManager.Instance != null ? GameManager.Instance.CurrentSpeed : forwardSpeed;
        transform.position += Vector3.forward * speed * Time.fixedDeltaTime;
    }
    
    private void ChangeLane()
    {
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 currentPos = transform.position;
        Vector3 newPos = new Vector3(
            Mathf.Lerp(currentPos.x, targetX, laneChangeSpeed * Time.fixedDeltaTime),
            currentPos.y,
            currentPos.z
        );
        transform.position = newPos;
    }
    
    public void MoveLeft()
    {
        if (currentLane > 0 && !isSliding)
        {
            currentLane--;
        }
    }
    
    public void MoveRight()
    {
        if (currentLane < numberOfLanes - 1 && !isSliding)
        {
            currentLane++;
        }
    }
    
    public void Jump()
    {
        if (isGrounded && !isSliding)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            
            if (animator != null)
            {
                animator.SetTrigger(jumpHash);
            }
            
            if (jumpParticles != null)
            {
                jumpParticles.Play();
            }
        }
    }
    
    private void StartSlide()
    {
        if (isGrounded && !isSliding)
        {
            isSliding = true;
            slideTimer = 0f;
            
            // Adjust collider for sliding
            if (capsuleCollider != null)
            {
                capsuleCollider.height = originalColliderHeight * slideHeight;
                capsuleCollider.center = new Vector3(
                    originalColliderCenter.x,
                    originalColliderCenter.y - (originalColliderHeight * (1 - slideHeight) / 2),
                    originalColliderCenter.z
                );
            }
            
            if (animator != null)
            {
                animator.SetTrigger(slideHash);
            }
            
            if (slideParticles != null)
            {
                slideParticles.Play();
            }
        }
    }
    
    private void UpdateSliding()
    {
        if (isSliding)
        {
            slideTimer += Time.deltaTime;
            
            if (slideTimer >= slideDuration)
            {
                EndSlide();
            }
        }
    }
    
    private void EndSlide()
    {
        isSliding = false;
        slideTimer = 0f;
        
        // Restore collider
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }
    }
    
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Additional check using raycast
        if (!isGrounded)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Game over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    public bool IsSliding => isSliding;
    public bool IsGrounded => isGrounded;
}

