using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private EnemyMovementHandler movementHandler;
    private EnemyAnimationHandler animationManager;

    private float chaseSpeed;
  
    private bool isInPlayerRange = false;

    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }

    public EnemyChaseState(EnemyController enemyController,EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController,stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.movementHandler = enemyController.EnemyMovementHandler;
        this.animationManager = enemyController.EnemyAnimationHandler;

        chaseSpeed = Random.Range(enemyConfig.minChaseSpeed, enemyConfig.maxChaseSpeed + 0.1f);
       
    }

    
    public override void EnterState()
    {
        animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void ExitState()
    {
        movementHandler.StopMove();
        animationManager.SetBoolForAnimation("isRunning", false);       
    }

    public override void FrameUpdate()
    {
        if (enemyController == null || enemyController.PlayerGO == null || enemyController.PlayerController.IsDead || enemyController.IsDead || stateMachine.AttackState.NextAttack == null || movementHandler.IsCurrentCellBlockedByEnemies())          
        {
            stateMachine.ChangeState(EnemyStateEnum.Idle);
            return;
        }
        isInPlayerRange = stateMachine.AttackState.IsEnemyInAttackRange();
        if (isInPlayerRange)
        {
            stateMachine.ChangeState(EnemyStateEnum.Attack);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!isInPlayerRange)
        {
            movementHandler.MoveToPlayer(ChaseSpeed);
        }
    }

  
}
