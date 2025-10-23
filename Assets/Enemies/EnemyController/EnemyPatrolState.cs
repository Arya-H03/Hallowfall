using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class EnemyPatrolState : EnemyState
{
    private EnemyMovementHandler movementHandler;
    private float patrolSpeed;
    private CellGrid patrolCellGrid = null;
    public EnemyPatrolState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.movementHandler = enemyController.EnemyMovementHandler;
        patrolSpeed = Random.Range(enemyConfig.minChaseSpeed, enemyConfig.maxChaseSpeed + 0.1f) / 2;
    }

    public CellGrid PatrolCellGrid { get => patrolCellGrid; set => patrolCellGrid = value; }

    public override void EnterState()
    {
        enemyController.SignalHub.OnAnimBool?.Invoke("isRunning", true);
    }

    public override void ExitState()
    {
        enemyController.SignalHub.OnAnimBool?.Invoke("isRunning", false);
    }

    public override void FrameUpdate()
    {
        if (enemyController == null ||  enemyController.IsDead)
        {
            stateMachine.ChangeState(EnemyStateEnum.Idle);
            return;
        }
        else if (enemyController.HasSeenPlayer)
        {
            stateMachine.ChangeState(EnemyStateEnum.Chase);
        }
    }

    public override void PhysicsUpdate()
    {
        if (patrolCellGrid.GetCellFromWorldPos(enemyController.GetEnemyPos()).TotalCost == 0) enemyController.SignalHub.OnAnimBool?.Invoke("isRunning", false);
        else if (enemyController.CanMove)
        {
            movementHandler.MoveToPatrol(patrolSpeed, patrolCellGrid);
        }
        
    }
}
