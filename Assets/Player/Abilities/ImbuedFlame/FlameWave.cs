using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWave : BaseProjectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().OnEnemyHit(damage, collision.transform.position, HitSfxType.fire,0);
        }
    }
}
