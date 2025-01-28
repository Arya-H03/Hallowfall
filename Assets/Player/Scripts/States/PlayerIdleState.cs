using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState()
    {
        this.stateEnum = PlayerStateEnum.Idle;
    }
    public override void OnEnterState()
    {
        playerController.AnimationController.SetBoolForAnimations("isFalling", false);
    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {


    }

   
}
