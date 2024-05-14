using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyController : MonoBehaviour
{

    private float maxHealth = 100;
    private float currentHealth;

    
    public EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;

    public EnemyStateEnum previousStateEnum;
    private EnemyBaseState previousState;

    private DialogueBox dialogueBox;
    private ParticleSystem bloodParticles;

    [HideInInspector]
    public EnemyBaseState idleState;
    [HideInInspector]
    public EnemyBaseState patrolState;
    [HideInInspector]
    public EnemyBaseState chaseState;
    [HideInInspector]
    public EnemyBaseState attackState;
    [HideInInspector]
    public EnemyBaseState stunState;
    [HideInInspector]
    public EnemyBaseState jumpState;
    [HideInInspector]
    public EnemyBaseState turnState;

    [HideInInspector]
    public SmartEnemyAgent agent;
    [HideInInspector]
    public EnemyAnimationManager animationManager;
    [HideInInspector]
    public EnemyMovement enemyMovement;
    [HideInInspector]
    public EnemyCollisionManager collisionManager;

    [HideInInspector]
    public GameObject player;

    [SerializeField] public GameObject stunEffect;

    public bool hasSeenPlayer = false;
    public bool canAttack = false;
    public bool isStuned = false;
    public bool isJumping = false;

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
            case EnemyStateEnum.Stun:
                return stunState;
            case EnemyStateEnum.Jump:
                return jumpState;
            case EnemyStateEnum.Turn:
                return turnState;
             
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

            previousState = currentState;
            previousStateEnum = currentStateEnum;

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
                case EnemyStateEnum.Stun:
                    currentState = stunState;
                    break;
                case EnemyStateEnum.Jump:
                    currentState = jumpState;
                    break;
                case EnemyStateEnum.Turn:
                    currentState = turnState;
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
        idleState.SetStatesController(this);

        patrolState = gameObject.AddComponent<PatrolState>();
        patrolState.SetStatesController(this);

        chaseState = gameObject.AddComponent<ChaseState>();
        chaseState.SetStatesController(this);

        attackState = gameObject.AddComponent<AttackState>();
        attackState.SetStatesController(this);

        stunState = gameObject.AddComponent<StunState>();
        stunState.SetStatesController(this);

        jumpState = gameObject.AddComponent<JumpState>();
        jumpState.SetStatesController(this);

        turnState = gameObject.AddComponent<TurnState>();
        turnState.SetStatesController(this);

        animationManager = GetComponent<EnemyAnimationManager>();
        enemyMovement = GetComponent<EnemyMovement>();
        collisionManager = GetComponent<EnemyCollisionManager>();
        agent = GetComponent<SmartEnemyAgent>();
        dialogueBox = GetComponentInChildren<DialogueBox>();
        bloodParticles = GetComponent<ParticleSystem>();    

        currentStateEnum = EnemyStateEnum.Idle;

        currentState = idleState;
    }
    private void Start()
    {
        ChangeState(EnemyStateEnum.Patrol);
            
    }

    private void Update()
    {
        handleCooldowns();

        //if(player && player.GetComponent<PlayerController>().isAttacking && jumpState.GetComponent<JumpState>().canJump && !isJumping)
        //{
        //    jumpState.GetComponent<JumpState>().canJump = false;
        //    int randomNumber = UnityEngine.Random.Range(1, 101);
        //    if (randomNumber <= 100 * 0.33)
        //    {
        //        ChangeState(EnemyStateEnum.Jump);
        //    }
        //    else
        //    {
        //        jumpState.GetComponent<JumpState>().canJump = true;
        //    }
          
        //}

        currentState.HandleState();

        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(EnemyStateEnum.Jump);
        }

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    ChangeState(EnemyStateEnum.Patrol);
        //}

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    ChangeState(EnemyStateEnum.Idle);
        //}
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

    public DialogueBox GetDialogueBox()
    {
        return dialogueBox; 
    }

    public void PlayBloodEffect(Vector2 hitPoint)
    {
        Vector2 distance = hitPoint - new Vector2(this.transform.position.x, this.transform.position.y);
        //Debug.Log(distance);
        var shape = bloodParticles.shape;
        shape.position = (distance);
        bloodParticles.Play();
    }


}
