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
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        statesManager.animationManager.SetBoolForAnimation("isTurning", false);
        statesManager.isStuned = true;
        statesManager.stunEffect.SetActive(true);
    }

    public override void OnExitState()
    {
        stunTimer = 0f;
        statesManager.isStuned = false;
        statesManager.stunEffect.SetActive(false);
    }

    public override void HandleState()
    {
       if(stunTimer < stunDuration)
        {
            stunTimer += Time.deltaTime;

        }

       else if(stunTimer >= stunDuration)
        {
            statesManager.ChangeState(EnemyStateEnum.Chase);
           
        }
        
    }
}
