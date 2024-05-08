using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : EnemyBaseState
{
    public JumpState() : base()
    {
        stateEnum = EnemyStateEnum.Jump;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        statesManager.animationManager.SetBoolForAnimation("isTurning", false);


    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {


    }



}
