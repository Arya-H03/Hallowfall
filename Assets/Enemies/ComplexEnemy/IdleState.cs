using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAI;

public class IdleState : EnemyBaseState
{
    private float patrolDelay = 0f;
    private float patrolDelayTimer = 0f;
    public IdleState() : base()
    {
        stateEnum = EnemyStateEnum.Idle;
        
    }

    private void Start()
    {
        RandomizePatrolDelay();
    }

    public override void OnEnterState()
    {
        RandomizePatrolDelay();
    }
    
    public override void OnExitState()
    {
        patrolDelayTimer = 0f;
    }

    public override void HandleState()
    {

        if(patrolDelayTimer < patrolDelay)
        {
            patrolDelayTimer += Time.deltaTime;
        }

        else if(patrolDelayTimer >= patrolDelay)
        {
            
            statesManager.ChangeState(EnemyStateEnum.Patrol);

        }
    }

    private void RandomizePatrolDelay()
    {
        patrolDelay = Random.Range(2, 5);
    }

   
}
