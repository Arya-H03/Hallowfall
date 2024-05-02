using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyBaseState
{
    private float chaseSpeed = 2f;
    public ChaseState() : base()
    {
        stateEnum = EnemyStateEnum.Chase;
       
    }

    public override void OnEnterState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
    }

    public override void HandleState()
    {
        if (statesManager.hasSeenPlayer)
        {
            if(Vector2.Distance(statesManager.player.transform.position, gameObject.transform.position) < 1.25f)
            {
                //statesManager.ChangeState(EnemyStateEnum.Attack);
                statesManager.canAttack = true;
            }

            else
            {
                statesManager.canAttack = false;
            }
            
            statesManager.enemyMovement.MoveTo(transform.position, statesManager.player.transform.position, chaseSpeed);
        }
        
    }
}
