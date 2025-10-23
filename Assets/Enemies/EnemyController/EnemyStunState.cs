using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyState
{
    private float stunDuration;
    private float stunTimer = 0f;
    private EnemyPhysicsHandler collisionManager;
    private EnemyAnimationHandler animationManager;

    public float StunDuration { get => stunDuration; set => stunDuration = value; }

    public EnemyStunState(EnemyController enemyController,EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.stunDuration = enemyConfig.stunDuration;
        this.collisionManager = enemyController.EnemyPhysicsHandler;
        this.animationManager = enemyController.EnemyAnimationHandler;
    }
   
    public override void EnterState()
    {
        //collisionManager.Rb.bodyType = RigidbodyType2D.Static;
        enemyController.SignalHub.OnAnimBool?.Invoke("isRunning", false);
        enemyController.SignalHub.OnAnimBool?.Invoke("isAttacking", false);
 
        enemyController.IsStuned = true;
        enemyController.stunEffect.SetActive(true);
    }

    public override void ExitState()
    {
        stunTimer = 0f;
        enemyController.IsStuned = false;
        enemyController.stunEffect.SetActive(false);
        //collisionManager.Rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void FrameUpdate()
    {
        if (stunTimer < StunDuration)
        {
            stunTimer += Time.deltaTime;

        }

        else if (stunTimer >= StunDuration)
        {

            stateMachine.ChangeState(EnemyStateEnum.Idle);

        }

    }

    public override void PhysicsUpdate()
    {
        return;
    }
}
