using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState : EnemyBaseState
{
    public TurnState() : base()
    {
        stateEnum = EnemyStateEnum.Turn;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {
        enemyController.animationManager.SetBoolForAnimation("isTurning", true);
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);


    }

    public override void OnExitState()
    {
        enemyController.animationManager.SetBoolForAnimation("isTurning", false);
        enemyController.animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void HandleState()
    {


    }


}
