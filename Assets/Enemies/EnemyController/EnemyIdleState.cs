using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyIdleState : EnemyState
{
    private EnemyMovementHandler movementHandler;
    public EnemyIdleState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        movementHandler = enemyController.EnemyMovementHandler;
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
            stateMachine.ChangeState(EnemyStateEnum.Chase);
        }
        else if(CanGoToPatrollState())
        {
            stateMachine.ChangeState(EnemyStateEnum.Patrol);
        }
    }
       
    

    public override void PhysicsUpdate()
    {
        return;
    }

    private bool CanGoToChaseState()
    {
        return
               !enemyController.PlayerController.IsDead &&
               enemyController.CanMove &&
               !enemyController.IsBeingknocked &&
               enemyController.HasSeenPlayer &&
                stateMachine.AttackState.CanChasePlayerToAttack();
    }

    private bool CanGoToPatrollState()
    {
        return
               !enemyController.PlayerController.IsDead &&
               enemyController.CanMove &&
               !enemyController.IsBeingknocked &&
               !enemyController.HasSeenPlayer &&
                stateMachine.PatrolState.PatrolCellGrid != null;
    }

}
