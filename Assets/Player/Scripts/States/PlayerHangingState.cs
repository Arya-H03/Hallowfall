using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHangingState : PlayerBaseState
{
    public PlayerHangingState()
    {
        this.stateEnum = PlayerStateEnum.Hang;
    }
    public override void OnEnterState()
    {
        playerController.AnimationController.SetBoolForAnimations("isHanging", true);
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;
        playerController.InputManager.DisablePlayerMovement();
    }

    public override void OnExitState()
    {
        playerController.AnimationController.SetBoolForAnimations("isHanging", false);
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;
        playerController.InputManager.EnablePlayerMovement();
    }

    public override void HandleState()
    {


    }

   
}
