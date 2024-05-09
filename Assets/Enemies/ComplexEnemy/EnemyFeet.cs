using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFeet : MonoBehaviour
{
    private EnemyStatesManager enemyStatesManager;

    private void Awake()
    {
        enemyStatesManager = GetComponentInParent<EnemyStatesManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if( collision.CompareTag("Ground") && enemyStatesManager.isJumping) 
        {
            enemyStatesManager.jumpState.GetComponent<JumpState>().OnGroundReached();
            
        }
    }
}
