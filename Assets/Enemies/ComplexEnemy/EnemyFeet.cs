using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFeet : MonoBehaviour
{
    private EnemyController enemyStatesManager;

    private void Awake()
    {
        enemyStatesManager = GetComponentInParent<EnemyController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //if( collision.CompareTag("Ground") && enemyStatesManager.isJumping) 
        //{
        //    enemyStatesManager.JumpState.OnGroundReached();
            
        //}
    }
}
