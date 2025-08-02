using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) :base(playerController, stateMachine,playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Idle;
    }
    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {

    }
}
