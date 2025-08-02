using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Death Explosion", menuName = "Enemy Death Explosion")]
public class ExplodingDeath : EnemyAbilitySO
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] GameObject attackZonePrefab;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] float damage = 20;

    private GameObject attackZoneGO;

    public override void ApplyAbility(EnemyController enemyController)
    {
        enemyController.EnemyStateMachine.DeathState.EnemyEndDeathEvent += () => CreateExplosion(enemyController);
        enemyController.EnemyStateMachine.DeathState.EnemyBeginDeathEvent += () => CreateAttackZone(enemyController);
    }

    private void CreateAttackZone(EnemyController enemyController)
    {
        attackZoneGO = Instantiate(attackZonePrefab, enemyController.transform.position,Quaternion.identity);
        attackZoneGO.transform.parent = enemyController.transform;
    }
    private void CreateExplosion(EnemyController enemyController)
    {
        Instantiate(explosionVFX, enemyController.GetEnemyPos(), Quaternion.identity);
        AudioManager.Instance.PlaySFX(explosionSFX, enemyController.transform.position, 1);
            
        if(attackZoneGO)
        {
            EnemyAttackZone attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
            //if (attackZone != null && attackZone.Target)
            //{
            //    attackZone.Target.GetComponent<PlayerController>().TryHitPlayer(damage);
            //}

            
        }

        Destroy(attackZoneGO);


    }


}
