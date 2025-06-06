using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        if(enemyController.AttackState.NextAttack && enemyController.AttackState.IsEnemyAbleToAttack())
        {
            enemyController.ChangeState(EnemyStateEnum.Chase);
        }
        
    }



   
}
