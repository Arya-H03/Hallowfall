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
            ;    
            collision.GetComponentInParent<EnemyController>().SignalHub.OnEnemyHit?.Invoke(playerController.PlayerConfig.dashAttackDamage, HitSfxType.sword, playerController.GetPlayerPos(), 2);
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
