using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
   
    private float chaseSpeed;
  
    private bool isInPlayerRange = false;
    EnemyAttackTypeEnum attackType;

    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }

    public EnemyChaseState(EnemyController enemyController, EnemyStateEnum stateEnum, EnemyConfigSO enemyConfig) : base(enemyController, stateEnum,enemyConfig)
    {
        chaseSpeed = Random.Range(enemyConfig.minChaseSpeed, enemyConfig.maxChaseSpeed + 0.1f);
    }

    
    public override void EnterState()
    {
        
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void ExitState()
    {
        enemyController.EnemyMovementHandler.StopMove();
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);       
    }

    public override void FrameUpdate()
    {
        if (enemyController == null || enemyController.Player == null || enemyController.PlayerController.IsDead || enemyController.IsDead || enemyController.AttackState.NextAttack == null || enemyController.EnemyMovementHandler.IsCurrentCellBlockedByEnemies())          
        {
            enemyController.ChangeState(EnemyStateEnum.Idle);
            return;
        }
        isInPlayerRange = enemyController.AttackState.IsEnemyInAttackRange();
        if (isInPlayerRange)
        {
            attackType = enemyController.AttackState.NextAttack.AttackTypeEnum;
            enemyController.ChangeState(EnemyStateEnum.Attack);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!isInPlayerRange)
        {
            enemyController.EnemyMovementHandler.MoveToPlayer(ChaseSpeed);
        }
    }

  
}
