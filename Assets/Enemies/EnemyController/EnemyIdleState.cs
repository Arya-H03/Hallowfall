using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyIdleState : EnemyState
{
    public EnemyIdleState (EnemyController enemyController, EnemyStateEnum stateEnum,EnemyConfigSO enemyConfig) :base(enemyController,stateEnum, enemyConfig)
    {

    }

    public override void EnterState()
    {
            
    }
    
    public override void ExitState()
    {
        
    }

    public override void FrameUpdate()
    {
        if (CanGoToChaseState())
        {
            enemyController.ChangeState(EnemyStateEnum.Chase);
        }
        
    }

    private bool CanGoToChaseState()
    {
        return enemyController.AttackState.NextAttack &&
               !enemyController.PlayerController.IsDead && 
               enemyController.CanMove && 
               !enemyController.IsBeingknocked &&
                enemyController.AttackState.IsEnemyAbleToAttack() &&
               !enemyController.EnemyMovementHandler.IsCurrentCellBlockedByEnemies();
    }

   
}
