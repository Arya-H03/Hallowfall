using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    #region Variables
    public float maxHealth = 100;
    public float currentHealth;
    private int enemyLvl = 1;

    private float damageModifier = 1;

    private FloorTypeEnum currentFloorType;


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


    [HideInInspector]
    private EnemyAnimationManager enemyAnimationManager;
    private EnemyMovement enemyMovement;
    [HideInInspector]
    public EnemyCollisionManager collisionManager;


    private GameObject player;
    public Vector3 playerPos;
    private PlayerController playerController;

    [SerializeField] public GameObject stunEffect;
    [SerializeField] DamagePopUp damagePopUp;

    private NavMeshAgent navAgent;
    private AudioSource audioSource;

    public bool hasSeenPlayer = false;
    public bool canAttack = false;
    public bool isStuned = false;
    public bool isJumping = false;
    private bool canChangeState = true;
    private bool canMove = true;
    private bool isDead = false;
    private bool isFacingLedge = false;
    private bool isPlayerDead = false;

    [SerializeField] EnemyAbilitySO[] enemyAbilities; 

    [SerializeField] Transform healthbarFG; 
    [SerializeField] Transform worldCanvas; 
    

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
    public bool CanMove { get => canMove; set => canMove = value; }
    public EnemyAnimationManager EnemyAnimationManager { get => enemyAnimationManager; set => enemyAnimationManager = value; }
    public EnemyDeathState DeathState { get => deathState; set => deathState = value; }
    public EnemyStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public EnemyBaseState CurrentState1 { get => currentState; set => currentState = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public bool IsFacingLedge { get => isFacingLedge; set => isFacingLedge = value; }
    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }
    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public float DamageModifier { get => DamageModifier1; set => DamageModifier1 = value; }
    public NavMeshAgent NavAgent { get => navAgent; set => navAgent = value; }
    public ParticleSystem BloodParticles { get => bloodParticles; set => bloodParticles = value; }
    public float DamageModifier1 { get => damageModifier; set => damageModifier = value; }
    public Material Material { get => material; set => material = value; }
    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }
    public Vector3 PlayerPos { get => playerPos; set => playerPos = value; }
    public GameObject Player { get => player; set => player = value; }
    public Transform HealthbarFG { get => healthbarFG; set => healthbarFG = value; }
    public Transform WorldCanvas { get => worldCanvas; set => worldCanvas = value; }
    public int EnemyLvl { get => enemyLvl; set => enemyLvl = value; }

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
        dialogueBox = GetComponentInChildren<DialogueBox>();
        BloodParticles = GetComponent<ParticleSystem>();
        Material = GetComponent<SpriteRenderer>().material;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        navAgent = GetComponent<NavMeshAgent>();
        AudioSource = GetComponent<AudioSource>();

        InitializeEnemyState();
    }
    private void Start()
    {
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        Player = GameManager.Instance.Player;
        playerController = Player.GetComponent<PlayerController>();
        playerPos = Player.transform.position + new Vector3(0,Player.GetComponent<SpriteRenderer>().sprite.bounds.size.y/2,0);

        currentHealth = maxHealth;
       
        hasSeenPlayer = true;

        foreach (var ability in enemyAbilities)
        {
            ability.ApplyAbility(this);
        }
        CurrentStateEnum = EnemyStateEnum.Idle;
        CurrentState = IdleState;

    }

    private void Update()
    {
        playerPos = Player.transform.position + new Vector3(0, Player.GetComponent<SpriteRenderer>().sprite.bounds.size.y / 2, 0);
        if (!isDead)
        {
            HandleCooldowns();
            canAttack = attackState.IsEnemyAbleToAttaack();
            CurrentState.HandleState();
        }
      

    }

    private void HandleCooldowns()
    {
        PatrolState.GetComponent<PatrolState>().ManagePatrolDelayCooldown();
    }

    public void OnEnemyHit(float damage, Vector2 hitPoint, HitSfxType hitType)
    {
        StartCoroutine(collisionManager.EnemyHitCoroutine(damage, hitPoint, hitType));
    }

    
    public void OnEnemyTakingDamage(float value,float modifier)
    {
        if (!isDead)
        {
            float damage = value * modifier;
            currentHealth -= damage;
            SpawnDamagePopUp(damage);
            Vector3 healthBarScale = new Vector3(currentHealth / maxHealth, 1, 1);
            healthbarFG.localScale = healthBarScale;

            if (currentHealth <= 0)
            {
               currentHealth = 0;
               
               ChangeState(EnemyStateEnum.Death);
               
            }
        }
      
    }

    private void SpawnDamagePopUp(float damage)
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

  

    public void SetCanChangeState(bool value)
    {
        this.canChangeState = value;
    }

    public bool GetCanChangeState()
    {
        return this.canChangeState;
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
    }

    public void ResetPlayer()
    {
        
        Player = null;
        hasSeenPlayer = false;
        isPlayerDead = true;
        ChangeState(EnemyStateEnum.Idle);
    }

}
