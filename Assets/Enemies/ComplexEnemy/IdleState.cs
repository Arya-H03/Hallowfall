using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAI;

public class IdleState : EnemyBaseState
{
    PlayerController playerController;
    public IdleState() : base()
    {
        stateEnum = EnemyStateEnum.Idle;
        
    }

    private void Start()
    {
        playerController = enemyController.player.GetComponentInChildren<PlayerController>();
    }

    public override void OnEnterState()
    {
               
    }
    
    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {
        if (enemyController.hasSeenPlayer && !playerController.IsDead)
        {
            if (!enemyController.IsFacingLedge)
            {
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }
            else if (enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x)
            {
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }

        }

        //else 
        ////{
        ////    enemyController.ChangeState(EnemyStateEnum.Patrol);
        ////}
        
    }



   
}
