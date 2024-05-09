using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : EnemyBaseState
{
    private float jumpSpeedY = 15;
    private float jumpSpeedX = 10;
    private int jumpDirectionX = 0;
    public JumpState() : base()
    {
        stateEnum = EnemyStateEnum.Jump;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        statesManager.animationManager.SetBoolForAnimation("isTurning", false);

        statesManager.isJumping = true;
        statesManager.collisionManager.ApplyVelocity(jumpSpeedX * jumpDirectionX, jumpSpeedY);
        statesManager.collisionManager.SetColliderIsTrigger(true);
    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {
       

    }

    public void OnGroundReached()
    {
        statesManager.collisionManager.SetColliderIsTrigger(false);
        statesManager.isJumping = false;
        statesManager.ChangeState(statesManager.previousStateEnum);
    }

    public void SetJumpDirectionX(int dir)
    {
        jumpDirectionX = dir;
    }


}
