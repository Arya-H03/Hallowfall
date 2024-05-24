using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    private PlayerFootSteps playerFootSteps;

    public PlayerFootSteps PlayerFootSteps {set => playerFootSteps = value; }

    [SerializeField] private float runSpeed = 3.5f;

    public PlayerRunState()
    {
        this.stateEnum = PlayerStateEnum.Run;
    }

    public void SetOnInitializeVariables(PlayerController statesManagerRef, PlayerFootSteps playerFootStepsRef)
    {
        this.playerController = statesManagerRef;
        PlayerFootSteps = playerFootStepsRef;
    }
    public override void OnEnterState()
    {
        
        StartRunning();
    }

    public override void OnExitState()
    {
        StopRunning();
    }

    public override void HandleState()
    {


    }

    private void StartRunning()
    {
        playerController.PlayerMovementManager.MoveSpeed = runSpeed;
        playerController.AnimationController.SetBoolForAnimations("isRunning", true);
        playerFootSteps.OnStartPlayerFootstep();

    }

    private void StopRunning()
    {
        playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        playerFootSteps.OnEndPlayerFootstep();

    }
}
