using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectiles : BaseProjectile
{
    [SerializeField] private HitSfxType hitSfxType;

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().SignalHub.OnEnemyHit?.Invoke(damage, hitSfxType, this.transform.position, 0);
            Destroy(this.gameObject);
        }
    }
}
