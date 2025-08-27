using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDeathExplosion", menuName = "Scriptable Objects/Enemy Behaviors/Death Explosion")]
public class ExplodingDeath : EnemyBehaviorSO
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] GameObject attackZonePrefab;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] float damage = 20;

    private GameObject attackZoneGO;
    private EnemyController enemyController;

    public override void InitBehavior(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        enemyController.SignalHub.OnEnemyDeathEnd += CreateExplosion;
        enemyController.SignalHub.OnEnemyDeathBegin += CreateAttackZone;
    }

    private void CreateAttackZone()
    {
        attackZoneGO = Instantiate(attackZonePrefab, enemyController.GetEnemyPos(),Quaternion.identity);
        attackZoneGO.transform.parent = enemyController.transform;
    }
    private void CreateExplosion()
    {
        Instantiate(explosionVFX, enemyController.GetEnemyPos(), Quaternion.identity);
        enemyController.SignalHub.OnPlaySFX?.Invoke(explosionSFX, 0.25f);
            
        if(attackZoneGO)
        {
            EnemyAttackZone attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
            if (attackZone != null)
            {
                attackZone.TryHitTarget(enemyController);
                //attackZone.Target.GetComponent<PlayerController>().TryHitPlayer(damage);
            }


        }

        Destroy(attackZoneGO);


    }


}
