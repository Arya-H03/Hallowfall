using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : BaseProjectile
{
    private GameObject enemyOwner;

    public GameObject EnemyOwner { get => enemyOwner; set => enemyOwner = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("PlayerGO"))
        {
            collision.GetComponent<PlayerController>().PlayerSignalHub.OnPlayerHit?.Invoke(damage);
        }
    }
}
