using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemyController : MonoBehaviour
{
    #region Variables
    private float maxHealth = 100;
    public float currentHealth;

    
    public EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;

    public EnemyStateEnum previousStateEnum;
    private EnemyBaseState previousState;

    private DialogueBox dialogueBox;
    private ParticleSystem bloodParticles;

    private IdleState idleState;
    private PatrolState patrolState;
    private ChaseState chaseState;
    private SwordAttackState swordAttackState;
    private StunState stunState;
    private JumpState jumpState;
    private TurnState turnState;
    private BlockState blockState;

    [HideInInspector]
    public SmartEnemyAgent agent;
    [HideInInspector]
    public EnemyAnimationManager animationManager;
    private EnemyMovement enemyMovement;
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
    private bool canMove = true;

    #endregion





    #region Getters / Setters
    public EnemyMovement EnemyMovement { get => enemyMovement; set => enemyMovement = value; }

    //Eenemy States
    public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
    public EnemyBaseState PreviousState { get => previousState; set => previousState = value; }
    public PatrolState PatrolState { get => patrolState; set => patrolState = value; }
    public IdleState IdleState { get => idleState; set => idleState = value; }
    public ChaseState ChaseState { get => chaseState; set => chaseState = value; }
    public SwordAttackState SwordAttackState { get => swordAttackState; set => swordAttackState = value; }
    public StunState StunState { get => stunState; set => stunState = value; }
    public JumpState JumpState { get => jumpState; set => jumpState = value; }
    public TurnState TurnState { get => turnState; set => turnState = value; }
    public BlockState BlockState { get => blockState; set => blockState = value; }

   
    public bool CanMove { get => canMove; set => canMove = value; }

    #endregion

    public EnemyBaseState GetState(EnemyStateEnum stateEnum)
    {
        switch (stateEnum)
        {
            default: return null;

            case EnemyStateEnum.Idle:
                return IdleState;            
            case EnemyStateEnum.Patrol:
                return PatrolState;
            case EnemyStateEnum.Chase:
                return ChaseState;
            case EnemyStateEnum.SwordAttack:
                return SwordAttackState;
            case EnemyStateEnum.Stun:
                return StunState;
            case EnemyStateEnum.Jump:
                return JumpState;
            case EnemyStateEnum.Turn:
                return TurnState;
            case EnemyStateEnum.Block:
                return BlockState;
             
        }
    }

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if(currentStateEnum != stateEnum && canChangeState)
        {
            Debug.Log(GetCurrentStateEnum().ToString() + " to " + stateEnum.ToString());

            if (CurrentState != null)
            {
                CurrentState.OnExitState();
            }

            PreviousState = CurrentState;
            previousStateEnum = currentStateEnum;

            switch (stateEnum)
            {
                case EnemyStateEnum.Idle:
                    CurrentState = IdleState;
                    break;
                case EnemyStateEnum.Patrol:
                    CurrentState = PatrolState;
                    break;
                case EnemyStateEnum.Chase:
                    CurrentState = ChaseState;
                    break;
                case EnemyStateEnum.SwordAttack:
                   
                    CurrentState = SwordAttackState;
                    break;
                case EnemyStateEnum.Stun:
                    CurrentState = StunState;
                    break;
                case EnemyStateEnum.Jump:
                    CurrentState = JumpState;
                    break;
                case EnemyStateEnum.Turn:
                    CurrentState = TurnState;
                    break;
                case EnemyStateEnum.Block:
                    if (GetCanBlock())
                    {
                        CurrentState = BlockState;
                    }
                    break;
            }

            currentStateEnum = stateEnum;
            CurrentState.OnEnterState();
        }
        
    }

    public void SetCurrentState(EnemyBaseState newState)
    {
        CurrentState = newState;
    }

    public EnemyBaseState GetCurrentState()
    {
        return CurrentState;    
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

        
        animationManager = GetComponent<EnemyAnimationManager>();
        EnemyMovement = GetComponent<EnemyMovement>();
        collisionManager = GetComponent<EnemyCollisionManager>();
        agent = GetComponent<SmartEnemyAgent>();
        dialogueBox = GetComponentInChildren<DialogueBox>();
        bloodParticles = GetComponent<ParticleSystem>();

        InitializeEnemyState();

        currentStateEnum = EnemyStateEnum.Idle;
        CurrentState = IdleState;


    }
    private void Start()
    {


        ChangeState(EnemyStateEnum.Patrol);

    }

    private void Update()
    {
        handleCooldowns();

        if (hasSeenPlayer)
        {
            if(Vector2.Distance(player.transform.position,this.transform.position) < swordAttackState.AttackRange && SwordAttackState.CanSwordAttack && !SwordAttackState.IsSwordAttaking)
            {
                ChangeState(EnemyStateEnum.SwordAttack);
            }

            else
            {
                if (CanMove && !isTurning)
                {
                    MoveToPlayer(3);
                }
                
            }

        }
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

        CurrentState.HandleState();

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
        PatrolState.GetComponent<PatrolState>().ManagePatrolDelayCooldown();
        BlockState.GetComponent<BlockState>().ManageBlockCooldown();
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

    public void MoveToPlayer(float speed)
    {
        EnemyMovement.MoveTo(this.gameObject.transform.position, player.transform.position, speed);
    }

    private void InitializeEnemyState()
    {
        IdleState = GetComponentInChildren<IdleState>();
        IdleState.SetStatesController(this);

        PatrolState = GetComponentInChildren<PatrolState>();
        PatrolState.SetStatesController(this);

        //ChaseState = GetComponentInChildren<ChaseState>();
        //ChaseState.SetStatesController(this);

        SwordAttackState = GetComponentInChildren<SwordAttackState>();
        SwordAttackState.SetStatesController(this);

        StunState = GetComponentInChildren<StunState>();
        StunState.SetStatesController(this);

        JumpState = GetComponentInChildren<JumpState>();
        JumpState.SetStatesController(this);

        TurnState = GetComponentInChildren<TurnState>();
        TurnState.SetStatesController(this);

        BlockState = GetComponentInChildren<BlockState>();
        BlockState.SetStatesController(this);
    }
}
