using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    [SerializeField] private float moveSpeedWhileParrying = 2;

    [SerializeField] GameObject parryShield;

    public PlayerParryState()
    {
        this.stateEnum = PlayerStateEnum.Parry;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileParrying;
        StartParry();
    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {


    }

    private void StartParry()
    {
        //playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        //playerController.AnimationController.SetBoolForAnimations("isJumping", false);


        if (playerController.IsPlayerJumping)
        {
            playerController.rb.gravityScale = 0.2f;
            playerController.rb.velocity = Vector2.zero;
        }

        playerController.IsParrying = true;
        playerController.AnimationController.SetTriggerForAnimations("Roll");
       
        
    }

    public void ActivateParryShield()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = true;
    }


    public void OnParryEnd()
    {

        parryShield.GetComponent<BoxCollider2D>().enabled = false;
        playerController.rb.gravityScale = 3;
        playerController.IsParrying = false;
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
