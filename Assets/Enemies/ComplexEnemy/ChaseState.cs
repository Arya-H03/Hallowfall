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
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
    }

    public override void HandleState()
    {
        if (enemyController.hasSeenPlayer)
        {
            if (Vector2.Distance(enemyController.player.transform.position, this.transform.position) < enemyController.AttackState.AttackRef.AttackRange)
            {
                enemyController.ChangeState(EnemyStateEnum.Attack);
                enemyController.canAttack = true;
            }

            else
            {
                enemyController.canAttack = false;
                if (enemyController.player.GetComponent<PlayerController>().IsHanging)
                {
                   enemyController.ChangeState(EnemyStateEnum.Idle);
                }
                else
                {
                    enemyController.EnemyMovement.MoveTo(transform.position, enemyController.player.transform.position, chaseSpeed);
                }
               
            }

            
        }

    }
}
