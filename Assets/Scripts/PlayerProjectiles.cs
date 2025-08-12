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
            collision.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = this.Damage, HitSfx = hitSfxType, AttackerPosition = this.transform.position, KnockbackForce = 0 });
            Destroy(this.gameObject);
        }
    }
}
