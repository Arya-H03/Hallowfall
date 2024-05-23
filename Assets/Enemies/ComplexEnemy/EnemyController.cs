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
    public float currentHealth;

    
    public EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;

    public EnemyStateEnum previousStateEnum;
    private EnemyBaseState previousState;

    private DialogueBox dialogueBox;
    private ParticleSystem bloodParticles;

    [HideInInspector]
    private EnemyBaseState idleState;
    [HideInInspector]
    private EnemyBaseState patrolState;
    [HideInInspector]
    private EnemyBaseState chaseState;
    [HideInInspector]
    private EnemyBaseState combatState;
    [HideInInspector]
    private EnemyBaseState stunState;
    [HideInInspector]
    private EnemyBaseState jumpState;
    [HideInInspector]
    private EnemyBaseState turnState;
    [HideInInspector]
    private EnemyBaseState blockState;

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
    private bool isTurning = false;
    private bool canChangeState = true;
    private bool canBlock = true;

    public EnemyBaseState GetState(EnemyStateEnum stateEnum)
    {
        switch (stateEnum)
        {
            default: return null;

            case EnemyStateEnum.Idle:
                return idleState;            
            case EnemyStateEnum.Patrol:
                return patrolState;
            case EnemyStateEnum.Chase:
                return chaseState;
            case EnemyStateEnum.Combat:
                return combatState;
            case EnemyStateEnum.Stun:
                return stunState;
            case EnemyStateEnum.Jump:
                return jumpState;
            case EnemyStateEnum.Turn:
                return turnState;
            case EnemyStateEnum.Block:
                return blockState;
             
        }
    }

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if(currentStateEnum != stateEnum && canChangeState)
        {
            //Debug.Log(GetCurrentStateEnum().ToString() + " to " + stateEnum.ToString());

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
                case EnemyStateEnum.Combat:
                    currentState = combatState;
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
                case EnemyStateEnum.Block:
                    if (GetCanBlock())
                    {
                        currentState = blockState;
                    }
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
        InitialzieEnemyState();

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

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeState(EnemyStateEnum.Block);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeState(EnemyStateEnum.Idle);
        }
    }

    private void handleCooldowns()
    {
        combatState.GetComponent<CombatState>().ManageSwordAttackCooldown();
        patrolState.GetComponent<PatrolState>().ManagePatrolDelayCooldown();
        blockState.GetComponent<BlockState>().ManageBlockCooldown();
    }

    public void OnEnemyDamage(float value)
    {
        if (currentHealth > 0)
        {
            currentHealth -= value;
       
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //agent.AddReward(-3);
                //agent.EndEpisode();  
                Debug.Log("I died");

            }
        }
        else
        {
            //agent.AddReward(-3);
            //agent.EndEpisode();
           Debug.Log("You are dead");
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
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

    public void SetIsTurning(bool value)
    {
        this.isTurning = value;
    }

    public bool GetIsTurning()
    {
        return this.isTurning;
    }

    public void SetCanChangeState(bool value)
    {
        this.canChangeState = value;
    }

    public bool GetCanChangeState()
    {
        return this.canChangeState;
    }

    public void SetCanBlock(bool value)
    {
        this.canBlock = value;
    }

    public bool GetCanBlock()
    {
        return this.canBlock;
    }



    private void InitialzieEnemyState()
    {
        idleState = gameObject.AddComponent<IdleState>();
        idleState.SetStatesController(this);

        patrolState = gameObject.AddComponent<PatrolState>();
        patrolState.SetStatesController(this);

        chaseState = gameObject.AddComponent<ChaseState>();
        chaseState.SetStatesController(this);

        combatState = gameObject.AddComponent<CombatState>();
        combatState.SetStatesController(this);

        stunState = gameObject.AddComponent<StunState>();
        stunState.SetStatesController(this);

        jumpState = gameObject.AddComponent<JumpState>();
        jumpState.SetStatesController(this);

        turnState = gameObject.AddComponent<TurnState>();
        turnState.SetStatesController(this);


        blockState = gameObject.AddComponent<BlockState>();
        blockState.SetStatesController(this);
    }
}
