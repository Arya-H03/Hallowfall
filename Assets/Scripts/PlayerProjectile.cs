using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerProjectile : BaseProjectile
{
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyCollider"))
        {
            collision.transform.parent.GetComponent<IHitable>().HandleHit(new HitInfo
            {
                damage = Damage,
                canBeImmune = false,
                canFlashOnHit = true,
                canPlayAnimOnHit = true,
                canPlaySFXOnHit = true,
                canPlayVFXOnHit = true,
            });
            PierceCount--;
            if(PierceCount <= 0 ) Destroy(this.gameObject);

        }
    }
}
