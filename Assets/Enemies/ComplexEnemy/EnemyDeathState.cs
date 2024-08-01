using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    public EnemyDeathState() : base()
    {
        stateEnum = EnemyStateEnum.Death;

    }

  

    public override void OnEnterState()
    {
        enemyController.IsDead = true;  
        enemyController.EnemyAnimationManager.SetTriggerForAnimation("Death");
    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {
        
    }

}
