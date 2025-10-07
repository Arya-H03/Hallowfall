using System.Collections;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHitHandler : MonoBehaviour, IDamagable, IInitializeable<EnemyController>, IHitable
{
    private EnemyController enemyController;
    private EnemySignalHub signalHub;


    private bool canStagger = true;
    private float maxStagger;
    private float currentStagger = 0;

    public bool CanStagger { get => canStagger; set => canStagger = value; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.signalHub = enemyController.SignalHub;

        MaxHealth = enemyController.EnemyConfig.maxHealth;
        RestoreFullHealth();
        DamageModifier = enemyController.EnemyConfig.damageModifier;
        maxStagger = enemyController.EnemyConfig.maxStagger;

        signalHub.OnRestoreFullHealth += RestoreFullHealth;
        signalHub.OnRestoreHealth += RestoreHealth;

    }

    //private void OnDisable()
    //{
    //    if (signalHub == null) return;

    //    signalHub.OnRestoreFullHealth -= RestoreFullHealth;
    //    signalHub.OnRestoreHealth -= RestoreHealth;
    //}

    public bool HandleHit(HitInfo hitInfo)
    {
        if (enemyController.IsDead) return false;

        if(hitInfo.canPlaySFXOnHit) signalHub.OnPlayHitSFX?.Invoke (0.5f);
        
        ApplyDamage((int)(hitInfo.damage * DamageModifier));

        if(hitInfo.canPlayAnimOnHit) signalHub.OnAnimTrigger?.Invoke("Hit");
        if(hitInfo.canFlashOnHit) signalHub.OnEnemyFlash?.Invoke(0.15f, Color.white);
        if (hitInfo.canPlayVFXOnHit) signalHub.OnPlayBloodEffect?.Invoke();

       
        if (hitInfo.knockbackInfo.canKnockback)
        {
            Vector2 hitDir = (enemyController.GetEnemyPos() - hitInfo.knockbackInfo.forceSourcePosition).normalized;
            signalHub.OnEnemyKnockBack?.Invoke(hitDir, hitInfo.knockbackInfo.knockbackForce);
        }    

        return true;
    }

 
    public void ApplyDamage(int amount)
    {
        if (enemyController.IsDead) return;
        CurrentHealth -= amount;
   
        TryStagger(amount);

        if (CurrentHealth <= 0) 
        {
            CurrentHealth = 0;
            Die(); 
        }
        signalHub.OnEnemyHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }

    public void RestoreFullHealth()
    {
        CurrentHealth = MaxHealth;
        signalHub.OnEnemyHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }

    private void RestoreHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        signalHub.OnEnemyHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }

    public bool TryStagger(float damageTaken)
    {
        if (currentStagger < maxStagger && canStagger)
        {
            currentStagger += (2 * damageTaken);

            if (currentStagger >= maxStagger)
            {
                currentStagger = 0;
                canStagger = false;

                StartCoroutine(EnemyStaggerCoroutine());
            }
        }
        return canStagger;
    }

    private IEnumerator EnemyStaggerCoroutine()
    {

        enemyController.EnemyStateMachine.ChangeState(EnemyStateEnum.Stun);

        yield return new WaitForSeconds(enemyController.EnemyConfig.timeBetweenStaggers);
        canStagger = true;
    }
    public void Die()
    {
        CurrentHealth = 0;
        enemyController.EnemyStateMachine.ChangeState(EnemyStateEnum.Death);
    }
}
