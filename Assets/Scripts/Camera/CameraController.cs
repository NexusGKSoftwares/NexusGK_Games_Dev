using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Player transform
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Camera Settings")]
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private bool lookAtTarget = true;
    
    private Vector3 velocity = Vector3.zero;
    private Transform cameraTransform;
    
    private void Awake()
    {
        cameraTransform = transform;
        
        // Find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        
        // Smoothly follow the target
        if (smoothFollow)
        {
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                desiredPosition,
                ref velocity,
                1f / followSpeed
            );
        }
        else
        {
            cameraTransform.position = desiredPosition;
        }
        
        // Look at the target
        if (lookAtTarget)
        {
            Vector3 lookDirection = target.position - cameraTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            cameraTransform.rotation = Quaternion.Slerp(
                cameraTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

