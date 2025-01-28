using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    [SerializeField] private AudioClip hitingGroundSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
    }
    public PlayerFallState()
    {
        this.stateEnum = PlayerStateEnum.Fall;
    }
    public override void OnEnterState()
    {
        playerController.IsFalling = true;
        //if (playerController.PlayerCollision.Rb.bodyType != RigidbodyType2D.Dynamic)
        //{
        //    playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;
        //}
       
        playerController.AnimationController.SetBoolForAnimations("isFalling", true);
    }

    public override void OnExitState()
    {
        playerController.AnimationController.SetBoolForAnimations("isFalling", false);
        playerController.IsFalling = false;
        //playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;

    }

    public override void HandleState()
    {


    }

    public void OnPlayerGrounded()
    {
        
        playerController.AnimationController.SetBoolForAnimations("isFalling", false);
        playerController.IsPlayerGrounded = true;
        playerController.CanPlayerJump = true;

        AudioManager.Instance.PlaySFX(audioSource, hitingGroundSound);

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
