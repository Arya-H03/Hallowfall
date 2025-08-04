using System.Collections;
using UnityEngine;

public class EnemyHitHandler : MonoBehaviour,IDamagable,IInitializeable<EnemyController>
{
    private EnemyController enemyController;
    private EnemySignalHub signalHub;

  
    private bool canStagger = true;
    private float maxStagger;
    private float currentStagger = 0;

    public bool CanStagger { get => canStagger; set => canStagger = value; }
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.signalHub = enemyController.SignalHub;

        MaxHealth = enemyController.EnemyConfig.maxHealth;
        RestoreFullHealth();
        DamageModifier = enemyController.EnemyConfig.damageModifier;
        maxStagger = enemyController.EnemyConfig.maxStagger;
      

        signalHub.OnEnemyHit += HandleHit;
        signalHub.OnEnemyDamage += HandleDamage;
        signalHub.OnEnemyDeSpawn += RestoreFullHealth;

    }

    private void OnDisable()
    {
        if (signalHub == null) return;
        signalHub.OnEnemyHit -= HandleHit;
        signalHub.OnEnemyDamage -= HandleDamage;
        signalHub.OnEnemyDeSpawn -= RestoreFullHealth;
    }

    private void HandleHit(float damage, HitSfxType hitSfx, Vector3 positionOfOther, float knockbackForce)
    {
        if (enemyController.IsDead) return;

        signalHub.OnAnimTrigger?.Invoke("Hit");
        signalHub.OnPlayHitSFX?.Invoke(hitSfx,0.5f);
        signalHub.OnEnemyDamage?.Invoke(damage);

        Vector2 hitDir = (enemyController.GetEnemyPos() - positionOfOther).normalized;
        signalHub.OnEnemyKnockBack?.Invoke(hitDir, knockbackForce);
    }

    private void HandleDamage(float value)
    {
        ApplyDamage(value * DamageModifier);
        TryStagger(value);
        signalHub.OnEnemyHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }
   
 

    public void ApplyDamage(float amount)
    {
        if (enemyController.IsDead) return;

        float damage = amount * DamageModifier;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) Die();
    }

    public void RestoreFullHealth()
    {
        CurrentHealth = MaxHealth;
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
