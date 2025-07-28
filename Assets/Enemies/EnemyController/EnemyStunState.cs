using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyState
{
    private float stunDuration;
    private float stunTimer = 0f;

    public float StunDuration { get => stunDuration; set => stunDuration = value; }

    public EnemyStunState(EnemyController enemyController, EnemyStateEnum stateEnum, EnemyConfigSO enemyConfig) : base(enemyController, stateEnum, enemyConfig)
    {
        this.stunDuration = enemyConfig.stunDuration;
    }

    public override void EnterState()
    {
        enemyController.CollisionManager.Rb.bodyType = RigidbodyType2D.Static;
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", false);      
        enemyController.isStuned = true;
        enemyController.stunEffect.SetActive(true);
    }

    public override void ExitState()
    {
        stunTimer = 0f;
        enemyController.isStuned = false;
        enemyController.stunEffect.SetActive(false);
        enemyController.CollisionManager.Rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void FrameUpdate()
    {
        if (stunTimer < StunDuration)
        {
            stunTimer += Time.deltaTime;

        }

        else if (stunTimer >= StunDuration)
        {

            enemyController.ChangeState(EnemyStateEnum.Idle);

        }

    }
}
