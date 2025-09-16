using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDeathExplosion", menuName = "Scriptable Objects/Enemy Behaviors/Death Explosion")]
public class ExplodingDeath : EnemyBehaviorSO
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] EnemyAttackZone attackZonePrefab;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] int damage = 20;

    private EnemyAttackZone attackZone;
    private EnemyController enemyController;

    public override void InitBehavior(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        enemyController.SignalHub.OnEnemyDeathEnd += CreateExplosion;
        enemyController.SignalHub.OnEnemyDeathBegin += CreateAttackZone;
    }

    private void CreateAttackZone()
    {
        attackZone = Instantiate(attackZonePrefab, enemyController.GetEnemyPos(),Quaternion.identity);
        attackZone.transform.parent = enemyController.transform;
        attackZone.transform.position = enemyController.transform.position;
        attackZone.Init(new EnemyMeleeStrikeData { owner = enemyController, strikeDamage = damage, parryDamage = 0 });
    }
    private void CreateExplosion()
    {
        Instantiate(explosionVFX, enemyController.GetEnemyPos(), Quaternion.identity);
        enemyController.SignalHub.OnPlaySFX?.Invoke(explosionSFX, 0.25f);
            
        if(attackZone)
        {
            attackZone.TryHitTarget(enemyController);
        }

        Destroy(attackZone.gameObject);


    }


}
