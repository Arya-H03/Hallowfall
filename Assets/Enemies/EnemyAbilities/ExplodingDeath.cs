using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Death Explosion", menuName = "Enemy Death Explosion")]
public class ExplodingDeath : EnemyAbilitySO
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] AudioClip explosionSFX;
    [SerializeField] float damage = 35;

    public override void ApplyAbility(EnemyController enemyController)
    {
        enemyController.DeathState.EnemyEndDeathEvent += () => CreateExplosion(enemyController);
    }

    private void CreateExplosion(EnemyController enemyController)
    {
        float spawnOffset = enemyController.SpriteRenderer.bounds.size.y / 2;
        Vector3 spawnPos = enemyController.transform.position;
        spawnPos.y += spawnOffset;
        GameObject vfx = Instantiate(explosionVFX, spawnPos, Quaternion.identity);
        AudioManager.Instance.PlaySFX(enemyController.AudioSource, explosionSFX);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(spawnPos, 1f, Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider != null)
            {
                if(hit.collider.CompareTag("Player"))
                {
                    hit.transform.GetComponent<PlayerController>().OnPlayerHit(damage);
                }

                else if (hit.collider.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<EnemyController>().OnEnemyHit(damage, hit.point,HitSfxType.sword);
                }
                   
  
            }
        }
    }


}
