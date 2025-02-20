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
        if (!enemyController.PlayerController.IsDead)
        {
            if(enemyController.AttackState.IsEnemyInAttackRange() && enemyController.AttackState.IsEnemyAbleToAttaack())
            {
                enemyController.ChangeState(EnemyStateEnum.Attack);
            }
            else if (!enemyController.AttackState.IsEnemyInAttackRange())
            {
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }
        }
        
    }



   
}
