using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateEnum
{
    None,
    Idle,
    Patrol,
    Chase,
    Attack,
    Stun,
    Hit,
    Death

}

public class EnemyState : IEntityState
{
    protected EnemyStateEnum stateEnum;
    protected EnemyController enemyController;
    protected EnemyConfigSO enemyConfig;
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyController enemyController,EnemyStateMachine stateMachine, EnemyStateEnum stateEnum)
    {
        this.stateEnum = stateEnum;       
        this.stateMachine = stateMachine;
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;

    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
}
