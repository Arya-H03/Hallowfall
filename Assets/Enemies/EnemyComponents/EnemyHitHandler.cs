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
        CurrentHealth = MaxHealth;
        DamageModifier = enemyController.EnemyConfig.damageModifier;
        maxStagger = enemyController.EnemyConfig.maxStagger;
      

        signalHub.OnEnemyHit += HandleHit;
        signalHub.OnEnemyDamage += HandleDamage;
        signalHub.OnEnemyDeSpawn += RestoreHealth;

    }

    private void OnDisable()
    {
        signalHub.OnEnemyHit -= HandleHit;
        signalHub.OnEnemyDamage -= HandleDamage;
        signalHub.OnEnemyDeSpawn -= RestoreHealth;
    }

    private void HandleHit(float damage, HitSfxType hitSfx)
    {
        signalHub.OnEnemyDamage?.Invoke(damage);
    }

    private void HandleDamage(float value)
    {
        ApplyDamage(value * DamageModifier);
        TryStagger(value);
        signalHub.OnEnemyHealthChange?.Invoke(MaxHealth, CurrentHealth);
    }
    public void HitEnemy(float damageAmount, HitSfxType hitType, float knockbackForce)
    {
        signalHub.OnEnemyHit?.Invoke(damageAmount, hitType);
    }

 

    public void ApplyDamage(float amount)
    {
        if (enemyController.IsDead) return;

        float damage = amount * DamageModifier;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) Die();
    }

    public void RestoreHealth()
    {
        CurrentHealth = MaxHealth;
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
