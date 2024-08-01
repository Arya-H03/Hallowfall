using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState : EnemyBaseState
{
    //[SerializeField] private bool isTurning = false;
    public TurnState() : base()
    {
        stateEnum = EnemyStateEnum.Turn;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {
        
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttackingSword", false);
        enemyController.SetIsTurning(true);
        enemyController.SetCanChangeState(false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isTurning", true);


    }

    public override void OnExitState()
    {
        
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isTurning", false);
        
    }

    public override void HandleState()
    {


    }



}
