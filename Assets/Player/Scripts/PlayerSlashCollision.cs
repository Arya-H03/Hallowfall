using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSlashCollision : MonoBehaviour
{
    private List<EnemyController> enemyTargets = new();
    private PlayerController playerController;
    public List<EnemyController> EnemyTargets { get => enemyTargets; }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerController && collision.CompareTag("EnemyCollider"))
        {
            EnemyController enemy = collision.transform.parent.gameObject.GetComponent<EnemyController>();
            if (!enemyTargets.Contains(enemy) && !enemy.IsDead)
            {
                enemyTargets.Add(enemy);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerController && collision.CompareTag("EnemyCollider"))
        {
            EnemyController enemy = collision.transform.parent.gameObject.GetComponent<EnemyController>();
            if (enemyTargets.Contains(enemy))
            {
                enemyTargets.Remove(enemy);
            }
        }
    }

}

