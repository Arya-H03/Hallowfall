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

        EnemyAttackZone attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
        if (attackZone != null && attackZone.Target)
        {
            attackZone.Target.GetComponent<PlayerController>().OnPlayerHit(damage);
        }

        Destroy(attackZoneGO);

        //RaycastHit2D[] hits = Physics2D.CircleCastAll(spawnPos, 1f, Vector2.zero);
        //foreach(RaycastHit2D hit in hits)
        //{
        //    if(hit.collider != null)
        //    {
        //        if(hit.collider.CompareTag("Player"))
        //        {
        //            hit.transform.GetComponent<PlayerController>().OnPlayerHit(damage);
        //        }

        //        else if (hit.collider.CompareTag("Enemy"))
        //        {
        //            hit.transform.GetComponent<EnemyController>().OnEnemyHit(damage, hit.point,HitSfxType.sword);
        //        }
                   
  
        //    }
        //}
    }


}
