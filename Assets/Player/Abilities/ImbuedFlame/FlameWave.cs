using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWave : BaseProjectile
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = this.Damage, HitSfx = HitSfxType.fire, AttackerPosition = this.transform.position, KnockbackForce = 0 });
        }
    }
}
