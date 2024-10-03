using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : EnemyBaseState
{
    private float stunDuration = 3f;
    private float stunTimer = 0f;

    
    public StunState() : base()
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
    }

    public override void HandleState()
    {
       if(stunTimer < stunDuration)
        {
            stunTimer += Time.deltaTime;

        }

       else if(stunTimer >= stunDuration)
        {
            //enemyController.ChangeState(enemyController.previousStateEnum);
            enemyController.ChangeState(EnemyStateEnum.Idle);
           
        }
        
    }
}
