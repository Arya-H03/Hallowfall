using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackBox : MonoBehaviour
{
    private PlayerController playerController;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag(playerController.EnemyTag))
        {
            collision.GetComponentInParent<IHitable>().HandleHit(new HitInfo { Damage = playerController.PlayerConfig.dashAttackDamage, HitSfx = HitSfxType.sword, AttackerPosition = playerController.GetPlayerPos(), KnockbackForce = 2, isImmuneable = false });
        }
    }

    public void EnableCollider()
    {
        boxCollider.enabled = true;
    }

    public void DisableCollider()
    {
        boxCollider.enabled = false;   
    }
}
