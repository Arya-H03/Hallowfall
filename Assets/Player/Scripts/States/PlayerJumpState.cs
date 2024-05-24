using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private AudioSource audioSource;
    [SerializeField] AudioClip jumpUpAC;
    [SerializeField] AudioClip groundHitAC;
    [SerializeField] float jumpSpeed;

    private float jumpDirectionX;

    public PlayerJumpState()
    {
        this.stateEnum = PlayerStateEnum.Jump;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
    }
    public override void OnEnterState()
    {
        playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        StartJump();
    }

    public override void OnExitState()
    {
        playerController.AnimationController.SetBoolForAnimations("isJumping", false);
    }

    public override void HandleState()
    {


    }
    private void StartJump()
    {
        playerController.IsPlayerJumping = true;
        playerController.IsPlayerGrounded = false;
        playerController.CanPlayerJump = false;
        playerController.CanPlayerAttack = false;

        jumpDirectionX = playerController.PlayerMovementManager.currentDirection.x;
        playerController.rb.gravityScale = 3;
        playerController.rb.velocity = new Vector2(jumpDirectionX * 5, jumpSpeed);

        playerController.GameManager.PlayAudio(audioSource, jumpUpAC);
        playerController.AnimationController.SetBoolForAnimations("isJumping", true);
    }

    public void EndJump()
    {
        playerController.IsPlayerGrounded = true;
        playerController.IsPlayerJumping = false;
        playerController.CanPlayerJump = true;

        playerController.GameManager.PlayAudio(audioSource, groundHitAC);
        playerController.AnimationController.SetBoolForAnimations("isJumping", false);

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
