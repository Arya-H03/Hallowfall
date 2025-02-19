using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    private AudioSource audioSource;
    private AudioClip rollSFX;
    private float rollCooldown = 0;
    private float rollModifier = 0;

    private Coroutine SpawnAfterImageCoroutine;
    public PlayerRollState()
    {
        this.stateEnum = PlayerStateEnum.Roll;
    }

    public override void InitState(PlayerConfig config)
    {
        rollSFX = config.rollSFX;
        rollCooldown = config.rollCooldown;
        rollModifier = config.rollModifier;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void OnEnterState()
    {
        StartCoroutine(BeginRollCoroutine());      
    }

    public override void OnExitState()
    {
        playerController.PlayerMovementManager.MoveSpeed = playerController.PlayerMovementManager.MoveSpeed;
        playerController.IsRolling = false;
    }

    public override void HandleState()
    {
     
    }

    private IEnumerator BeginRollCoroutine()
    {
        playerController.IsRolling = true;
        playerController.AnimationController.SetTriggerForAnimations("Roll");
        Vector2 dir = playerController.PlayerMovementManager.CurrentDirection;
        playerController.PlayerCollision.Rb.velocity += dir * rollModifier;
        SpawnAfterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
        AudioManager.Instance.PlaySFX(audioSource, rollSFX, 0.5f);
        playerController.PlayerMovementManager.MoveSpeed = 0;
        playerController.CanRoll = false;
        yield return new WaitForSeconds(0.5f);
        EndRoll();
        yield return new WaitForSeconds(rollCooldown);
        playerController.CanRoll = true;

    }
    
    public void EndRoll()
    {
        if (SpawnAfterImageCoroutine != null)
        {
            StopCoroutine(SpawnAfterImageCoroutine);
            SpawnAfterImageCoroutine = null;
        }
        playerController.IsRolling = false;
        playerController.PlayerCollision.Rb.velocity = Vector2.zero;
        if (playerController.CurrentStateEnum == PlayerStateEnum.Roll) 
        {
            if (playerController.PlayerMovementManager.currentInputDir.x != 0)
            {
                playerController.ChangeState(PlayerStateEnum.Run);
            }
            else
            {
                playerController.ChangeState(PlayerStateEnum.Idle);
            }
        }
       
    }
 
}
