using UnityEngine;

public class MovingEnvironment : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private bool useGameManagerSpeed = true;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector3 moveDirection = Vector3.back;
    
    private void Update()
    {
        if (!useGameManagerSpeed || GameManager.Instance == null || !GameManager.Instance.IsGameActive)
        {
            return;
        }
        
        // Move environment backward to create forward movement illusion
        float currentSpeed = useGameManagerSpeed ? GameManager.Instance.CurrentSpeed : speed;
        transform.position += moveDirection.normalized * currentSpeed * Time.deltaTime;
    }
}

