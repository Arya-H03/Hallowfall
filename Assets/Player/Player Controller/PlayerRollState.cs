using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class PlayerRollState : PlayerState
{
    private AudioClip rollSFX;
    private float rollCooldown = 0;
    private float rollModifier = 0;
    private float rollDuration;


    public PlayerRollState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Roll;
        rollSFX = playerConfig.rollSFX;
        rollCooldown = playerConfig.rollCooldown;
        rollModifier = playerConfig.rollModifier;
        rollDuration = playerConfig.rollDuration;
    }
    public override void EnterState()
    {
        playerController.IsRolling = true;
        playerController.IsImmune = true;
        playerController.CanRoll = false;

        signalHub.OnAnimTrigger?.Invoke("Roll");
        signalHub.OnApplyForwardVelocity?.Invoke(rollModifier);
        signalHub.OnAfterImageStart?.Invoke();
        signalHub.OnPlaySFX?.Invoke(rollSFX, 0.5f);

        playerController.CoroutineRunner.RunCoroutine(WaitingForRollEndCoroutine());

    }

    public override void ExitState()
    {
        playerController.IsRolling = false;
    }

    private IEnumerator WaitingForRollEndCoroutine()
    {
        yield return new WaitForSeconds(rollDuration);

        EndRoll();

        yield return new WaitForSeconds(rollCooldown);

        playerController.CanRoll = true;
        playerController.IsImmune = false;

    }
    private void EndRoll()
    {
        signalHub.OnAfterImageStop?.Invoke();
        signalHub.OnResetVelocity?.Invoke();
        signalHub.OnStateTransitionBasedOnMovement?.Invoke(stateEnum);
    }

}
