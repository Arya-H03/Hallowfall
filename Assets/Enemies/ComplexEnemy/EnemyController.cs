using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Components
    private EnemyAnimationManager enemyAnimationManager;
    private EnemyMovement enemyMovement;
    public EnemyCollisionManager collisionManager;
    private DialogueBox dialogueBox;
    private ParticleSystem bloodParticles;
    private Material material;
    private SpriteRenderer spriteRenderer;
    private EnemyItemDropHandler itemDropHandler;
    private NavMeshAgent navAgent;
    private AudioSource audioSource;

    // References
    private GameObject player;
    private PlayerController playerController;

    // States
    private EnemyIdleState idleState;
    private EnemyPatrolState patrolState;
    private EnemyChaseState chaseState;
    private EnemyAttackState attackState;
    private EnemyStunState stunState;
    private EnemyDeathState deathState;

    // Config
    [SerializeField] private EnemyTypeEnum enemyType;
    [SerializeField] private EnemyAbilitySO[] enemyAbilities;
    [SerializeField] private Transform healthbarFG;
    [SerializeField] private Transform worldCanvas;
    [SerializeField] private DamagePopUp damagePopUp;
    [SerializeField] public GameObject stunEffect;

    // Stats
    public float maxHealth = 100;
    public float currentHealth;
    private float damageModifier = 1;
    private int enemyLvl = 1;

    // State
    private FloorTypeEnum currentFloorType;
    [SerializeField] private EnemyStateEnum currentStateEnum;
    private EnemyBaseState currentState;
    public EnemyStateEnum previousStateEnum;
    private EnemyBaseState previousState;

    // Flags
    public bool hasSeenPlayer = false;
    public bool canAttack = false;
    public bool isStuned = false;
    public bool isJumping = false;
    private bool canChangeState = true;
    private bool canMove = true;
    private bool isDead = false;
    private bool isFacingLedge = false;
    private bool isPlayerDead = false;
    private bool isBeingknocked = false;

    // Runtime
    public Vector3 playerPos;

    #region Getters / Setters
    public EnemyMovement EnemyMovement { get => enemyMovement; set => enemyMovement = value; }
    public EnemyBaseState CurrentState { get => currentState; set => currentState = value; }
    public EnemyBaseState PreviousState { get => previousState; set => previousState = value; }
    public EnemyPatrolState PatrolState { get => patrolState; set => patrolState = value; }
    public EnemyIdleState IdleState { get => idleState; set => idleState = value; }
    public EnemyChaseState ChaseState { get => chaseState; set => chaseState = value; }
    public EnemyAttackState AttackState { get => attackState; set => attackState = value; }
    public EnemyStunState StunState { get => stunState; set => stunState = value; }
    public EnemyDeathState DeathState { get => deathState; set => deathState = value; }
    public EnemyAnimationManager EnemyAnimationManager { get => enemyAnimationManager; set => enemyAnimationManager = value; }
    public EnemyStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public bool IsFacingLedge { get => isFacingLedge; set => isFacingLedge = value; }
    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }
    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public float DamageModifier { get => damageModifier; set => damageModifier = value; }
    public NavMeshAgent NavAgent { get => navAgent; set => navAgent = value; }
    public ParticleSystem BloodParticles { get => bloodParticles; set => bloodParticles = value; }
    public Material Material { get => material; set => material = value; }
    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }
    public Vector3 PlayerPos { get => playerPos; set => playerPos = value; }
    public GameObject Player { get => player; set => player = value; }
    public Transform HealthbarFG { get => healthbarFG; set => healthbarFG = value; }
    public Transform WorldCanvas { get => worldCanvas; set => worldCanvas = value; }
    public int EnemyLvl { get => enemyLvl; set => enemyLvl = value; }
    public EnemyItemDropHandler ItemDropHandler { get => itemDropHandler; set => itemDropHandler = value; }
    public EnemyTypeEnum EnemyType { get => enemyType; }
    public bool CanChangeState { get => canChangeState; set => canChangeState = value; }
    public bool IsBeingknocked { get => isBeingknocked; set => isBeingknocked = value; }
    #endregion

    private void Awake()
    {
        EnemyAnimationManager = GetComponent<EnemyAnimationManager>();
        EnemyMovement = GetComponent<EnemyMovement>();
        collisionManager = GetComponent<EnemyCollisionManager>();
        dialogueBox = GetComponentInChildren<DialogueBox>();
        BloodParticles = GetComponent<ParticleSystem>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Material = SpriteRenderer.material;
        navAgent = GetComponent<NavMeshAgent>();
        AudioSource = GetComponent<AudioSource>();
        itemDropHandler = GetComponentInChildren<EnemyItemDropHandler>();

        InitializeEnemyState();
    }

    private void Start()
    {
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        Player = GameManager.Instance.Player;
        playerController = Player.GetComponent<PlayerController>();
        playerPos = Player.transform.position;

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
        playerPos = Player.transform.position + new Vector3(0, Player.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);

        if (!isDead)
        {
            HandleCooldowns();
            canAttack = attackState.IsEnemyAbleToAttack();
            currentState.HandleState();
            attackState.CheckForNextAttack();
        }
    }

    private void HandleCooldowns()
    {
        PatrolState.ManagePatrolDelayCooldown();
    }

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if (CurrentStateEnum != stateEnum && canChangeState && !isDead)
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

    private void InitializeEnemyState()
    {
        IdleState = GetComponentInChildren<EnemyIdleState>();
        PatrolState = GetComponentInChildren<EnemyPatrolState>();
        ChaseState = GetComponentInChildren<EnemyChaseState>();
        AttackState = GetComponentInChildren<EnemyAttackState>();
        StunState = GetComponentInChildren<EnemyStunState>();
        DeathState = GetComponentInChildren<EnemyDeathState>();

        IdleState.SetStatesController(this);
        PatrolState.SetStatesController(this);
        ChaseState.SetStatesController(this);
        AttackState.SetStatesController(this);
        StunState.SetStatesController(this);
        DeathState.SetStatesController(this);
    }

    public IEnumerator EnemyHitCoroutine(float damage, Vector2 hitPoint, HitSfxType hitType,float knockbackForce)
    {
        //VFX
        collisionManager.PlayBloodEffect(hitPoint);
        Material.SetFloat("_Flash", 1);
        //SFX
        if (hitType != HitSfxType.none) AudioManager.Instance.PlaySFX( collisionManager.GetHitSound(hitType),transform.position, 0.4f);

        collisionManager.StaggerEnemy(damage);
        Vector2 knockbackVector = (GetEnemyCenter() - playerController.GetPlayerCenter()).normalized;
        collisionManager.KnockBackEnemy(knockbackVector, knockbackForce);

        //Damage
        OnEnemyTakingDamage(damage, DamageModifier);

        //Wait
        yield return new WaitForSeconds(0.1f);
        Material.SetFloat("_Flash", 0);


    }


    public void OnEnemyHit(float damage, Vector2 hitPoint, HitSfxType hitType, float knockbackForce)
    {
        StartCoroutine(EnemyHitCoroutine(damage, hitPoint, hitType, knockbackForce));
    }

    public void OnEnemyTakingDamage(float value, float modifier)
    {
        if (!isDead)
        {
            float damage = value * modifier;
            currentHealth -= damage;
            SpawnDamagePopUp(damage);
            UpdateEnemyHealthBar();

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                ChangeState(EnemyStateEnum.Death);
            }
        }
    }

    private void SpawnDamagePopUp(float damage)
    {
        DamagePopUp obj = Instantiate(damagePopUp, transform.position + Vector3.up, Quaternion.identity);
        obj.SetText(damage.ToString());
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void ResetEnemy()
    {
        switch (enemyType)
        {
            case EnemyTypeEnum.Arsonist:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(gameObject);
                break;
            case EnemyTypeEnum.Revenant:
                ObjectPoolManager.Instance.RevenantPool.ReturnToPool(gameObject);
                break;
            case EnemyTypeEnum.Sinner:
                ObjectPoolManager.Instance.SinnerPool.ReturnToPool(gameObject);
                break;
            case EnemyTypeEnum.Necromancer:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(gameObject);
                break;
        }

        IsDead = false;
        collisionManager.Rb.bodyType = RigidbodyType2D.Dynamic;
        collisionManager.BoxCollider.enabled = true;
        navAgent.enabled = true;
        worldCanvas.gameObject.SetActive(true);
        attackState.IsAttackDelayOver = true;
        enemyAnimationManager.Animator.enabled = true;

        ResetHealth();
        UpdateEnemyHealthBar();
        ChangeState(EnemyStateEnum.Idle);
    }

    public Vector3 GetEnemyCenter()
    {
        Vector3 center = transform.position;
        center.y += spriteRenderer.bounds.size.y / 2;
        return center;
    }

    public void UpdateEnemyHealthBar()
    {
        Vector3 scale = new Vector3(currentHealth / maxHealth, 1, 1);
        healthbarFG.localScale = scale;
    }
}
