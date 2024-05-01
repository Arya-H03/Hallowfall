using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAI;

public class IdleState : EnemyBaseState
{
    
    public IdleState() : base()
    {
        stateEnum = EnemyStateEnum.Idle;
        
    }

    private void Start()
    {
        
    }

    public override void OnEnterState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
        
        
    }
    
    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {

        
    }



   
}
