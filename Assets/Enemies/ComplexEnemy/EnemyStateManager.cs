using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyStatesManager : MonoBehaviour
{

    private float maxHealth = 100;
    private float currentHealth;

    private EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;


    private EnemyBaseState idleState;
    private EnemyBaseState patrolState;
    private EnemyBaseState chaseState;
    private EnemyBaseState attackState;

    public EnemyAnimationManager animationManager;
    public EnemyMovement enemyMovement;
    public EnemyCollisionManager collisionManager;

    public GameObject player;

    public bool hasSeenPlayer = false;

    public EnemyBaseState GetState(EnemyStateEnum stateEnum)
    {
        switch (stateEnum)
        {

            case EnemyStateEnum.Idle:
                return idleState;            
            case EnemyStateEnum.Patrol:
                return patrolState;
            case EnemyStateEnum.Chase:
                return chaseState;
            case EnemyStateEnum.Attack:
                return attackState;
        }

        return currentState;
    }

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
                case EnemyStateEnum.Attack:
                    currentState = attackState;
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

        attackState = gameObject.AddComponent<AttackState>();
        attackState.SetStatesManager(this);

        animationManager = GetComponent<EnemyAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
        collisionManager = GetComponent<EnemyCollisionManager>();

        currentStateEnum = EnemyStateEnum.Idle;

        currentState = idleState;
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        handleCooldowns();

        //switch (currentStateEnum)
        //{

        //    case EnemyStateEnum.Idle:
        //        idleState.HandleState();
        //        break;
        //    case EnemyStateEnum.Patrol:
        //        patrolState.HandleState();
        //        break;
        //    case EnemyStateEnum.Chase:
        //        chaseState.HandleState();
        //        break;
        //    case EnemyStateEnum.Attack:
        //        attackState.HandleState();
        //        break;
        //}

        currentState.HandleState();

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

    private void handleCooldowns()
    {
        attackState.GetComponent<AttackState>().ManageSwordAttackCooldown();
        patrolState.GetComponent<PatrolState>().ManagePatrolDelayCooldown();
    }

    public void OnEnemyDamage(float value)
    {
        if (currentHealth > 0)
        {
            currentHealth -= value;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Debug.Log("I died");

            }
        }
        else
        {
            //Debug.Log("You are dead");
        }
    }


}
