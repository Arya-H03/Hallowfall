using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStatesManager : MonoBehaviour
{


    private EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;


    private EnemyBaseState idleState;
    private EnemyBaseState patrolState;
    private EnemyBaseState chaseState;

    public EnemyAnimationManager animationManager;

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if(currentStateEnum != stateEnum)
        {
            Debug.Log(GetCurrentStateEnum().ToString() + " to " + stateEnum.ToString());

            if (currentState != null)
            {
                currentState.OnExitState();
            }

            switch (stateEnum)
            {

                case EnemyStateEnum.Idle:
                    currentState = idleState;
                    break;
                case EnemyStateEnum.Patrol:
                    currentState = patrolState;
                    break;
                case EnemyStateEnum.Chase:
                    currentState = chaseState;
                    break;
            }

            currentStateEnum = stateEnum;
            currentState.OnEnterState();
        }
        
    }

    public void SetCurrentState(EnemyBaseState newState)
    {
        currentState = newState;
    }

    public EnemyBaseState GetCurrentState()
    {
        return currentState;    
    }

    public void SetCurrentStateEnum(EnemyStateEnum newStateEnum)
    {
        currentStateEnum = newStateEnum;
    }

    public EnemyStateEnum GetCurrentStateEnum()
    {
        return currentStateEnum;
    }

    private void Awake()
    {
        idleState = gameObject.AddComponent<IdleState>();
        idleState.SetStatesManager(this);

        patrolState = gameObject.AddComponent<PatrolState>();
        patrolState.SetStatesManager(this);

        chaseState = gameObject.AddComponent<ChaseState>();
        chaseState.SetStatesManager(this);

        animationManager = GetComponent<EnemyAnimationManager>();

        currentStateEnum = EnemyStateEnum.Idle;

        currentState = idleState;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        
        switch (currentStateEnum)
        {
            
            case EnemyStateEnum.Idle:
                idleState.HandleState();
                break;
            case EnemyStateEnum.Patrol:
                patrolState.HandleState();
                break;
            case EnemyStateEnum.Chase:
                chaseState.HandleState();
                break;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeState(EnemyStateEnum.Chase);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(EnemyStateEnum.Patrol);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeState(EnemyStateEnum.Idle);
        }
    }


    
}
