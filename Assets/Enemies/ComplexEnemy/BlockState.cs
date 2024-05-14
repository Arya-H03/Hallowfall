using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : EnemyBaseState
{
    public BlockState() : base()
    {
        stateEnum = EnemyStateEnum.Block;

    }

    private void Start()
    {

    }

    public override void OnEnterState()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);

        enemyController.animationManager.SetTriggerForAnimation("Block");
        
        
    }

    public override void OnExitState()
    {
        enemyController.animationManager.SetBoolForAnimation("isBlocking", false);
    }

    public override void HandleState()
    {


    }

    public void BeginBlockingSword()
    {
        enemyController.animationManager.SetBoolForAnimation("isBlocking", true);
    }



}
