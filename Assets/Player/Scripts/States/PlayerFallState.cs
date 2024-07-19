using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState()
    {
        this.stateEnum = PlayerStateEnum.Fall;
    }
    public override void OnEnterState()
    {
        playerController.IsFalling = true;
        playerController.AnimationController.SetBoolForAnimations("isFalling", true);
    }

    public override void OnExitState()
    {
        playerController.AnimationController.SetBoolForAnimations("isFalling", false);
        playerController.IsFalling = false;

    }

    public override void HandleState()
    {


    }

    public void OnPlayerGrounded()
    {
        playerController.AnimationController.SetBoolForAnimations("isFalling", false);

        //playerController.GameManager.PlayAudio(audioSource, groundHitAC);

        if (playerController.PlayerMovementManager.currentDirection.x != 0)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
    }
}
