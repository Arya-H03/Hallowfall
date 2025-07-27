using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyBaseState
{
    private float stunDuration = 3f;
    private float stunTimer = 0f;

    public float StunDuration { get => stunDuration; set => stunDuration = value; }

    public EnemyStunState() : base()
    {
        stateEnum = EnemyStateEnum.Stun;
    }

    public override void OnEnterState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.isStuned = true;
        enemyController.stunEffect.SetActive(true);
    }

    public override void OnExitState()
    {
        stunTimer = 0f;
        enemyController.isStuned = false;
        enemyController.stunEffect.SetActive(false);
        enemyController.CollisionManager.ResetStagger();
    }

    public override void UpdateLogic()
    {
       if(stunTimer < StunDuration)
        {
            stunTimer += Time.deltaTime;

        }

       else if(stunTimer >= StunDuration)
        {
          
            enemyController.ChangeState(EnemyStateEnum.Idle);
           
        }
        
    }
}
