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
        enemyController.DeathState.EnemyEndDeathEvent += () => CreateExplosion(enemyController);
        enemyController.DeathState.EnemyBeginDeathEvent += () => CreateAttackZone(enemyController);
    }

    private void CreateAttackZone(EnemyController enemyController)
    {
        attackZoneGO = Instantiate(attackZonePrefab, enemyController.transform.position,Quaternion.identity);
    }
    private void CreateExplosion(EnemyController enemyController)
    {
        GameObject vfx = Instantiate(explosionVFX, enemyController.GetEnemyCenter(), Quaternion.identity);
        AudioManager.Instance.PlaySFX(enemyController.AudioSource, explosionSFX,1);
            
        if(attackZoneGO)
        {
            EnemyAttackZone attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
            if (attackZone != null && attackZone.Target)
            {
                attackZone.Target.GetComponent<PlayerController>().OnPlayerHit(damage);
            }

            Destroy(attackZoneGO);
        }
        

    }


}
