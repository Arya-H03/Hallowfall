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
        if(collision && collision.CompareTag("Enemy"))
        {
            Vector2 collisionPoint = collision.ClosestPoint(transform.position);
            collision.GetComponentInParent<EnemyController>().OnEnemyHit(playerController.PlayerDashState.DashAttackDamage, collisionPoint,HitSfxType.sword, 1);
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
