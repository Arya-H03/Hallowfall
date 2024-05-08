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
        statesManager.animationManager.SetBoolForAnimation("isTurning", true);
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);


    }

    public override void OnExitState()
    {
        statesManager.animationManager.SetBoolForAnimation("isTurning", false);
        statesManager.animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void HandleState()
    {


    }


}
