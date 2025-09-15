using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerHitHandler : MonoBehaviour, IInitializeable<PlayerController>, IDamagable,IHitable
{
    private PlayerController playerController;
    private PlayerConfig playerConfig;
    private PlayerSignalHub signalHub;
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set ; }
    public float DamageModifier { get; set; }

    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        playerConfig = playerController.PlayerConfig;
        signalHub = playerController.PlayerSignalHub;

        MaxHealth = playerConfig.maxHealth;
        DamageModifier = playerConfig.damageModifier;
        RestoreFullHealth();
 
        signalHub.OnRestoreHealth += RestoreHealth;   
        signalHub.OnRestoreFullHealth += RestoreFullHealth;   

        signalHub.MaxHealthBinding = new PropertyBinding<int> (() =>  MaxHealth, (value=> MaxHealth = value));
    }

    private void OnDisable()
    {
        signalHub.OnRestoreHealth -= RestoreHealth;
        signalHub.OnRestoreFullHealth -= RestoreFullHealth;
    }


    public void HandleHit(HitInfo hitInfo)
    {
        if (playerController.IsDead || (playerController.IsImmune && hitInfo.isImmuneable == true)) return;
        ApplyDamage(hitInfo.Damage);
        signalHub.OnPlaySFX?.Invoke(playerConfig.hitSFX,0.25f);
    }

    public void ApplyDamage(int amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
       
        signalHub.OnCameraShake?.Invoke(playerConfig.cameraShakeOnHitDuration, playerConfig.cameraShakeOnHitIntensity);
        signalHub.OnVignetteFlash?.Invoke(playerConfig.cameraShakeOnHitDuration, playerConfig.vignetteFlashOnHitIntensity,playerConfig.vignetteFlashOnHitColor);
        signalHub.OnMaterialFlash?.Invoke(playerConfig.cameraShakeOnHitDuration);
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth, CurrentHealth, -amount);
    }

    public void Die()
    {
        CurrentHealth = 0;
        signalHub.OnChangeState?.Invoke(PlayerStateEnum.Death);
    }



    public void RestoreFullHealth()
    {
        int changedAmount = (int)(MaxHealth - CurrentHealth);  
        CurrentHealth = MaxHealth;
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth,CurrentHealth, changedAmount);
    }

    public void RestoreHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        signalHub.OnPlayerHealthChange?.Invoke(MaxHealth, CurrentHealth, amount);
    }

   

    
}
