using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAI;

public class IdleState : EnemyBaseState
{
   
    public IdleState() : base()
    {
        stateEnum = EnemyStateEnum.Idle;
        
    }

    private void Start()
    {
        
    }

    public override void OnEnterState()
    {
               
    }
    
    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {
        if (enemyController.hasSeenPlayer && !enemyController.PlayerController.IsDead)
        {
            if (!enemyController.IsFacingLedge)
            {
                Debug.Log("Not FE");
                enemyController.ChangeState(EnemyStateEnum.Chase);
                
            }
            else if (enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x)
            {
                Debug.Log("other");
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }

        }

        //else 
        ////{
        ////    enemyController.ChangeState(EnemyStateEnum.Patrol);
        ////}
        
    }



   
}
