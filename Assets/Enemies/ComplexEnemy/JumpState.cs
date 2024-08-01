using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : EnemyBaseState
{
    private float jumpSpeedY = 15;
    private float jumpSpeedX = 10;
    private int jumpDirectionX = 0;
    public bool canJump = true;
    public JumpState() : base()
    {
        stateEnum = EnemyStateEnum.Jump;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {

        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttackingSword", false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isTurning", false);

        
        
       
            
            enemyController.isJumping = true;
            enemyController.collisionManager.ApplyVelocity(jumpSpeedX * jumpDirectionX, jumpSpeedY);
            enemyController.collisionManager.SetColliderIsTrigger(true);
        
       
    }

    public override void OnExitState()
    {
        canJump = true;
    }

    public override void HandleState()
    {
       

    }

    public void OnGroundReached()
    {
        canJump = true;
        enemyController.collisionManager.SetColliderIsTrigger(false);
        enemyController.isJumping = false;
        enemyController.ChangeState(enemyController.previousStateEnum);
        
    }

    public void SetJumpDirectionX(int dir)
    {
        jumpDirectionX = dir;
    }


}
