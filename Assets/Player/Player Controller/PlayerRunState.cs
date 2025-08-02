using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class PlayerRunState : PlayerState
{
    private float runSpeed;
    public PlayerRunState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Run;
        runSpeed = playerConfig.runSpeed;
    }

    public override void EnterState()
    {
        signalHub.OnChangeMoveSpeed?.Invoke(runSpeed);
        signalHub.OnAnimBool?.Invoke("isRunning",true);
        signalHub.OnAllowMovement?.Invoke(true);
    }

    public override void ExitState()
    {
        signalHub.OnAnimBool?.Invoke("isRunning", false);
        signalHub.OnAllowMovement?.Invoke(false);
    }

}
