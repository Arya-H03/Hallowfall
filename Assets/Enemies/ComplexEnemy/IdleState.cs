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
        if (enemyController.hasSeenPlayer && !enemyController.player.GetComponent<PlayerController>().IsHanging && enemyController.CurrentPlatformElevation.ElevationLevel == enemyController.player.GetComponent<PlayerController>().CurrentPlatformElevation.ElevationLevel)
        {
            enemyController.ChangeState(EnemyStateEnum.Chase);
        }
        
    }



   
}
