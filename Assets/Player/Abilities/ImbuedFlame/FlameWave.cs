using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWave : BaseProjectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().SignalHub.OnEnemyHit?.Invoke(damage, HitSfxType.fire,this.transform.position,0);
        }
    }
}
