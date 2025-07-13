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
            collision.GetComponent<EnemyController>().OnEnemyHit(damage, collision.transform.position, hitSfxType, 0    );
            Destroy(this.gameObject);
        }
    }
}
