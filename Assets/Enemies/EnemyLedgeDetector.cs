using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLedgeDetector : MonoBehaviour
{
    EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground") || collision.CompareTag("Grass"))
        {
            
            enemyController.IsFacingLedge = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Grass"))
        {
            enemyController.IsFacingLedge = true;

        }
    }
}
