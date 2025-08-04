using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private EnemySignalHub signalHub;

    public delegate void EventHandler();
    public EventHandler EnemyBeginDeathEvent;
    public EventHandler EnemyEndDeathEvent;
    public EnemyDeathState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;      
        signalHub = enemyController.SignalHub;
       
    }
    public override void EnterState()
    {
        enemyController.IsDead = true;

        signalHub.OnResetAnimTrigger?.Invoke("Hit");
        signalHub.OnAnimTrigger?.Invoke("Death");
        signalHub.OnEnemyDeath?.Invoke();
    }

}
