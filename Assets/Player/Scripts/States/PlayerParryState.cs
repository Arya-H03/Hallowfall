using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    [SerializeField] private float moveSpeedWhileParrying = 2;
    public PlayerParryState()
    {
        this.stateEnum = PlayerStateEnum.Parry;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileParrying;
    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {


    }
}
