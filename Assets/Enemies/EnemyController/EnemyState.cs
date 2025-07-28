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

    public EnemyState(EnemyController enemyController, EnemyStateEnum stateEnum, EnemyConfigSO enemyConfig)
    {
        this.enemyController = enemyController;
        this.stateEnum = stateEnum;
        this.enemyConfig = enemyConfig;
    }

    public EnemyState()
    {
    }

    public EnemyStateEnum GetStateEnum()
    {
        return stateEnum;
    }


    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
}
