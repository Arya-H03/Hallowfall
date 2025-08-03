using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHitHandler : MonoBehaviour, IInitializeable<PlayerController>, IDamagable
{
    private PlayerController playerController;
    private PlayerConfig playerConfig;
    private PlayerSignalHub signalHub;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set ; }
    public float DamageModifier { get; set; }

    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        playerConfig = playerController.PlayerConfig;
        signalHub = playerController.PlayerSignalHub;

        MaxHealth = playerConfig.maxHealth;
        DamageModifier = playerConfig.damageModifier;
        RestoreFullHealth();

        signalHub.OnPlayerHit += HandleHit;
        signalHub.OnPlayerDamage += HandleDamage;   
        signalHub.OnRestoreHealth += RestoreHealth;   
        signalHub.OnRestoreFullHealth += RestoreFullHealth;   

        signalHub.MaxHealthBinding = new PropertyBinding<float> (() =>  MaxHealth, (value=> MaxHealth = value));
    }

    private void OnDisable()
    {

        signalHub.OnPlayerHit -= HandleHit;
        signalHub.OnPlayerDamage -= HandleDamage;
        signalHub.OnRestoreHealth -= RestoreHealth;
        signalHub.OnRestoreFullHealth -= RestoreFullHealth;
    }


    private void HandleHit(float damage)
    {
        if (playerController.IsDead || playerController.IsImmune) return;
        signalHub.OnPlayerDamage?.Invoke(damage);
    }

    private void HandleDamage(float damage)
    {
        ApplyDamage(damage);
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth,CurrentHealth);
    }
    public void ApplyDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        CurrentHealth = 0;
        signalHub.OnChangeState?.Invoke(PlayerStateEnum.Death);
    }



    public void RestoreFullHealth()
    {
        CurrentHealth = MaxHealth;
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth,CurrentHealth);
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }

   

    
}
