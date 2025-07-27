using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyIdleState : EnemyBaseState
{
   
    public EnemyIdleState() : base()
    {
        stateEnum = EnemyStateEnum.Idle;
        
    }

    public override void OnEnterState()
    {
            
    }
    
    public override void OnExitState()
    {
        
    }

    public override void UpdateLogic()
    {
        if(enemyController.AttackState.NextAttack && enemyController.AttackState.IsEnemyAbleToAttack() && !enemyController.PlayerController.IsDead && enemyController.CanMove && !enemyController.IsBeingknocked && !enemyController.EnemyMovementHandler.IsCurrentCellBlockedByEnemies())
        {
            enemyController.ChangeState(EnemyStateEnum.Chase);
        }
        
    }



   
}
