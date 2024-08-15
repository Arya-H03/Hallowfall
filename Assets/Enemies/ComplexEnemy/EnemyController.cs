using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    #region Variables
    private float maxHealth = 100;
    public float currentHealth;


    [SerializeField] private EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;

    public EnemyStateEnum previousStateEnum;
    private EnemyBaseState previousState;

    private DialogueBox dialogueBox;
    private ParticleSystem bloodParticles;
    private Material material;
    private SpriteRenderer spriteRenderer;

    private IdleState idleState;
    private PatrolState patrolState;
    private ChaseState chaseState;
    private EnemyAttackState attackState;
    private StunState stunState;
    private EnemyDeathState deathState;
    private PlatformTag currentPlatformElevation;
    //private JumpState jumpState;
    //private TurnState turnState;
    //private BlockState blockState;
    //private EnemyRangeAttackState rangeAttackState;

    [HideInInspector]
    public SmartEnemyAgent agent;
    [HideInInspector]
    private EnemyAnimationManager enemyAnimationManager;
    private EnemyMovement enemyMovement;
    [HideInInspector]
    public EnemyCollisionManager collisionManager;

    [HideInInspector]
    public GameObject player;

    [SerializeField] public GameObject stunEffect;
    [SerializeField] DamagePopUp damagePopUp;

    public bool hasSeenPlayer = false;
    public bool canAttack = false;
    public bool isStuned = false;
    public bool isJumping = false;
    private bool isTurning = false;
    private bool canChangeState = true;
    private bool canBlock = true;
    private bool canMove = true;
    private bool isDead = false;
    private bool isInvincible = false;

    #endregion





    #region Getters / Setters
    public EnemyMovement EnemyMovement { get => enemyMovement; set => enemyMovement = value; }

    //Eenemy States
    public EnemyBaseState CurrentState { get => CurrentState1; set => CurrentState1 = value; }
    public EnemyBaseState PreviousState { get => previousState; set => previousState = value; }
    public PatrolState PatrolState { get => patrolState; set => patrolState = value; }
    public IdleState IdleState { get => idleState; set => idleState = value; }
    public ChaseState ChaseState { get => chaseState; set => chaseState = value; }
    public EnemyAttackState AttackState { get => attackState; set => attackState = value; }
    public StunState StunState { get => stunState; set => stunState = value; }
    //public JumpState JumpState { get => jumpState; set => jumpState = value; }
    //public TurnState TurnState { get => turnState; set => turnState = value; }
    //public BlockState BlockState { get => blockState; set => blockState = value; }

   
    public bool CanMove { get => canMove; set => canMove = value; }
    public EnemyAnimationManager EnemyAnimationManager { get => enemyAnimationManager; set => enemyAnimationManager = value; }
    public EnemyDeathState DeathState { get => deathState; set => deathState = value; }
    public EnemyStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public EnemyBaseState CurrentState1 { get => currentState; set => currentState = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public PlatformTag CurrentPlatformElevation { get => currentPlatformElevation; set => currentPlatformElevation = value; }

    //public EnemyRangeAttackState RangeAttackState { get => rangeAttackState; set => rangeAttackState = value; }

    #endregion

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if(CurrentStateEnum != stateEnum && canChangeState &&!isDead)
        {
            //Debug.Log(CurrentStateEnum.ToString() + " to " + stateEnum.ToString());

            if (CurrentState != null)
            {
                CurrentState.OnExitState();
            }

            PreviousState = CurrentState;
            previousStateEnum = CurrentStateEnum;

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
                case EnemyStateEnum.Attack:
                   
                    CurrentState = AttackState;
                    break;
                case EnemyStateEnum.Stun:
                    CurrentState = StunState;
                    break;
                case EnemyStateEnum.Death:
                    CurrentState = DeathState;
                    break;
            }

            CurrentStateEnum = stateEnum;
            CurrentState.OnEnterState();
        }
        
    }

   
  
    private void Awake()
    {


        EnemyAnimationManager = GetComponent<EnemyAnimationManager>();
        EnemyMovement = GetComponent<EnemyMovement>();
        collisionManager = GetComponent<EnemyCollisionManager>();
        agent = GetComponent<SmartEnemyAgent>();
        dialogueBox = GetComponentInChildren<DialogueBox>();
        bloodParticles = GetComponent<ParticleSystem>();
        material = GetComponent<SpriteRenderer>().material;
        SpriteRenderer = GetComponent<SpriteRenderer>();

        InitializeEnemyState();

        //CurrentStateEnum = EnemyStateEnum.Idle;
        //CurrentState = IdleState;




    }
    private void Start()
    {
        currentHealth = maxHealth;

        CurrentStateEnum = EnemyStateEnum.Idle;
        CurrentState = IdleState;

        ChangeState(EnemyStateEnum.Patrol);
    }

    private void Update()
    {
        HandleCooldowns();

        CurrentState.HandleState();

    }

    private void HandleCooldowns()
    {
        PatrolState.GetComponent<PatrolState>().ManagePatrolDelayCooldown();
    }

    public void OnEnemyHit(int damage, Vector2 hitPoint,GameObject playerRef)
    {
        if(!hasSeenPlayer )
        {
            if (playerRef)
            {
                this.player = playerRef;
            }
            
            hasSeenPlayer = true;
            if (CurrentStateEnum != EnemyStateEnum.Chase)
            {
                ChangeState(EnemyStateEnum.Chase);
            }
        }
        
        if (!isInvincible && damage > 0)
        {
            StartCoroutine(EnemyHitCoroutine(damage, hitPoint));
        }
       
    }

    private IEnumerator EnemyHitCoroutine(int damage, Vector2 hitPoint)
    {
        
        isInvincible = true;
        //material.SetFloat("_Flash", 1);
        PlayBloodEffect(hitPoint);
        SpawnDamagePopUp(damage);
        OnEnemyDamage(damage);
        yield return new WaitForSeconds(0.25f);
        //material.SetFloat("_Flash", 0);
        //SFX
        isInvincible = false;
    }
    public void OnEnemyDamage(float value)
    {
        if (currentHealth > 0)
        {
            
            currentHealth -= value;
       
            if (currentHealth <= 0)
            {
               currentHealth = 0;
               ChangeState(EnemyStateEnum.Death);

            }

            if(!isDead)
            {
                enemyAnimationManager.SetTriggerForAnimation("Hit");
            }
            
        }
        else
        {

            ChangeState(EnemyStateEnum.Death);
        }
    }

    private void SpawnDamagePopUp(int damage)
    {
        DamagePopUp obj = Instantiate(damagePopUp, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
        obj.SetText(damage.ToString());
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

        ChaseState = GetComponentInChildren<ChaseState>();
        ChaseState.SetStatesController(this);

        AttackState = GetComponentInChildren<EnemyAttackState>();
        AttackState.SetStatesController(this);

        StunState = GetComponentInChildren<StunState>();
        StunState.SetStatesController(this);

        DeathState = GetComponentInChildren<EnemyDeathState>();
        DeathState.SetStatesController(this);   

        //JumpState = GetComponentInChildren<JumpState>();
        //JumpState.SetStatesController(this);

        //TurnState = GetComponentInChildren<TurnState>();
        //TurnState.SetStatesController(this);

        //BlockState = GetComponentInChildren<BlockState>();
        //BlockState.SetStatesController(this);

        //RangeAttackState = GetComponentInChildren<EnemyRangeAttackState>();
        //RangeAttackState.SetStatesController(this);
    }
}
