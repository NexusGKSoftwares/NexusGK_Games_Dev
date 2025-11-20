using UnityEngine;
using System.Collections;
using System;

public class PowerUpManager : MonoBehaviour
{
    [Header("Power-Up Effects")]
    [SerializeField] private float speedBoostMultiplier = 1.5f;
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private float magnetPullSpeed = 10f;
    
    // Active power-ups
    private bool hasShield = false;
    private bool hasSpeedBoost = false;
    private bool hasMagnet = false;
    private bool hasDoubleCoins = false;
    
    // Events
    public event Action<PowerUpType, bool> OnPowerUpChanged;
    
    private Coroutine activePowerUpCoroutine;
    private PlayerController player;
    private float originalGameSpeed;
    
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    
    public void ActivatePowerUp(PowerUpType type, float duration)
    {
        // Stop any active power-up coroutine
        if (activePowerUpCoroutine != null)
        {
            StopCoroutine(activePowerUpCoroutine);
        }
        
        activePowerUpCoroutine = StartCoroutine(PowerUpCoroutine(type, duration));
    }
    
    private IEnumerator PowerUpCoroutine(PowerUpType type, float duration)
    {
        // Activate power-up
        switch (type)
        {
            case PowerUpType.Shield:
                hasShield = true;
                OnPowerUpChanged?.Invoke(PowerUpType.Shield, true);
                break;
                
            case PowerUpType.SpeedBoost:
                hasSpeedBoost = true;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CurrentSpeed *= speedBoostMultiplier;
                }
                OnPowerUpChanged?.Invoke(PowerUpType.SpeedBoost, true);
                break;
                
            case PowerUpType.Magnet:
                hasMagnet = true;
                OnPowerUpChanged?.Invoke(PowerUpType.Magnet, true);
                break;
                
            case PowerUpType.DoubleCoins:
                hasDoubleCoins = true;
                OnPowerUpChanged?.Invoke(PowerUpType.DoubleCoins, true);
                break;
        }
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Deactivate power-up
        switch (type)
        {
            case PowerUpType.Shield:
                hasShield = false;
                OnPowerUpChanged?.Invoke(PowerUpType.Shield, false);
                break;
                
            case PowerUpType.SpeedBoost:
                hasSpeedBoost = false;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CurrentSpeed /= speedBoostMultiplier;
                }
                OnPowerUpChanged?.Invoke(PowerUpType.SpeedBoost, false);
                break;
                
            case PowerUpType.Magnet:
                hasMagnet = false;
                OnPowerUpChanged?.Invoke(PowerUpType.Magnet, false);
                break;
                
            case PowerUpType.DoubleCoins:
                hasDoubleCoins = false;
                OnPowerUpChanged?.Invoke(PowerUpType.DoubleCoins, false);
                break;
        }
    }
    
    private void Update()
    {
        if (hasMagnet && player != null)
        {
            AttractCoins();
        }
    }
    
    private void AttractCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        
        foreach (GameObject coin in coins)
        {
            if (coin == null) continue;
            
            float distance = Vector3.Distance(player.transform.position, coin.transform.position);
            
            if (distance <= magnetRange)
            {
                // Pull coin toward player
                Vector3 direction = (player.transform.position - coin.transform.position).normalized;
                coin.transform.position += direction * magnetPullSpeed * Time.deltaTime;
            }
        }
    }
    
    public bool HasShield()
    {
        return hasShield;
    }
    
    public bool HasDoubleCoins()
    {
        return hasDoubleCoins;
    }
}

