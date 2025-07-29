using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CCoroutineRunner))]
public class EnemyController : MonoBehaviour, IDamagable
{
    #region Serialized Fields

    [Header("Data")]
    [SerializeField] private EnemyConfigSO enemyConfig;
    [SerializeField] private EnemyTypeEnum enemyType;
    [SerializeField] private EnemyAbilitySO[] enemyAbilities;

    [Header("UI")]
    [SerializeField] private Transform healthbarFG;
    [SerializeField] private Transform worldCanvas;

    [Header("Prefabs")]
    [SerializeField] private DamagePopUp damagePopUp;
    [SerializeField] public GameObject stunEffect;

    [Header("State")]
    [SerializeField] private EnemyStateEnum currentStateEnum;

    #endregion

    #region Components

    private EnemyAnimationManager enemyAnimationManager;
    private EnemyMovementHandler enemyMovementHandler;
    private EnemyCollisionManager collisionManager;
    private EnemyItemDropHandler itemDropHandler;
    private EnemyEnvironenmentCheck enemyEnvironenmentCheck;
    private CCoroutineRunner coroutineRunner;

    private ParticleSystem bloodParticles;
    private SpriteRenderer spriteRenderer;
    private Material material;

    #endregion

    #region State Machine

    private EnemyIdleState idleState;
    private EnemyChaseState chaseState;
    private EnemyAttackState attackState;
    private EnemyStunState stunState;
    private EnemyDeathState deathState;

    private EnemyState currentState;

    #endregion

    #region Runtime References

    private GameObject player;
    private PlayerController playerController;
    private List<EnemyBaseAttack> attacks;

    #endregion

    #region Flags

    public bool hasSeenPlayer = false;
    public bool canAttack = false;
    public bool isStuned = false;
    public bool isPlayerDead = false;
    public bool isBeingknocked = false;

    private bool canChangeState = true;
    private bool canMove = true;
    private bool isDead = false;

    #endregion

    #region Stats

    private int enemyLvl = 1;
    private FloorTypeEnum currentFloorType;

    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    #endregion

    #region Properties

    public EnemyMovementHandler EnemyMovementHandler { get => enemyMovementHandler; set => enemyMovementHandler = value; }
    public EnemyState CurrentState { get => currentState; set => currentState = value; }
    public EnemyIdleState IdleState { get => idleState; set => idleState = value; }
    public EnemyChaseState ChaseState { get => chaseState; set => chaseState = value; }
    public EnemyAttackState AttackState { get => attackState; set => attackState = value; }
    public EnemyStunState StunState { get => stunState; set => stunState = value; }
    public EnemyDeathState DeathState { get => deathState; set => deathState = value; }
    public EnemyAnimationManager EnemyAnimationManager { get => enemyAnimationManager; set => enemyAnimationManager = value; }
    public EnemyStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public Material Material { get => material; set => material = value; }
    public ParticleSystem BloodParticles { get => bloodParticles; set => bloodParticles = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public GameObject Player { get => player; set => player = value; }
    public Transform HealthbarFG { get => healthbarFG; set => healthbarFG = value; }
    public Transform WorldCanvas { get => worldCanvas; set => worldCanvas = value; }
    public int EnemyLvl { get => enemyLvl; set => enemyLvl = value; }
    public EnemyItemDropHandler ItemDropHandler { get => itemDropHandler; set => itemDropHandler = value; }
    public EnemyTypeEnum EnemyType => enemyType;
    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }
    public bool CanChangeState { get => canChangeState; set => canChangeState = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }
    public bool IsBeingknocked { get => isBeingknocked; set => isBeingknocked = value; }
    public EnemyCollisionManager CollisionManager { get => collisionManager; set => collisionManager = value; }
    public EnemyEnvironenmentCheck EnemyEnvironenmentCheck => enemyEnvironenmentCheck;
    public EnemyConfigSO EnemyConfig => enemyConfig;
    public CCoroutineRunner CoroutineRunner => coroutineRunner;
    public List<EnemyBaseAttack> Attacks => attacks;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        enemyAnimationManager = GetComponentInChildren<EnemyAnimationManager>();
        enemyMovementHandler = GetComponent<EnemyMovementHandler>();
        collisionManager = GetComponentInChildren<EnemyCollisionManager>();
        itemDropHandler = GetComponentInChildren<EnemyItemDropHandler>();
        enemyEnvironenmentCheck = GetComponentInChildren<EnemyEnvironenmentCheck>();
        coroutineRunner = GetComponent<CCoroutineRunner>();

        bloodParticles = GetComponent<ParticleSystem>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        material = spriteRenderer.material;

        attacks = GetComponents<EnemyBaseAttack>().ToList();
    }

    private void Start()
    {
        InitializeEnemyStats();
        InitializeEnemyStates();

        Player = GameManager.Instance.Player;
        playerController = Player.GetComponent<PlayerController>();
        hasSeenPlayer = true;

        foreach (var ability in enemyAbilities)
            ability.ApplyAbility(this);


        currentStateEnum = EnemyStateEnum.Idle;
        currentState = idleState;
    }

    private void Update()
    {
        if (isDead) return;

        canAttack = attackState.IsEnemyAbleToAttack();
        currentState.FrameUpdate();
        attackState.CheckForNextAttack();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
    }

    #endregion

    #region Initialization

    private void InitializeEnemyStates()
    {
        currentState = new();
        idleState = new EnemyIdleState(this, EnemyStateEnum.Idle, enemyConfig);
        chaseState = new EnemyChaseState(this, EnemyStateEnum.Chase, enemyConfig);
        attackState = new EnemyAttackState(this, EnemyStateEnum.Attack, enemyConfig);
        stunState = new EnemyStunState(this, EnemyStateEnum.Stun, enemyConfig);
        deathState = new EnemyDeathState(this, EnemyStateEnum.Death, enemyConfig);
    }

    private void InitializeEnemyStats()
    {
        MaxHealth = enemyConfig.maxHealth;
        CurrentHealth = MaxHealth;
        DamageModifier = enemyConfig.damageModifier;
    }

    #endregion

    #region State Management

    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if (currentStateEnum != stateEnum && canChangeState && !isDead)
        {
            //Debug.Log(CurrentStateEnum.ToString() + " to " + stateEnum.ToString());
            CurrentState?.ExitState();
            switch (stateEnum)
            {
                case EnemyStateEnum.Idle:
                    currentState = idleState;
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
                case EnemyStateEnum.Death:
                    currentState = deathState;
                    break;
            }
            currentStateEnum = stateEnum;
            currentState.EnterState();
        }
    }

    #endregion

    #region Damage & Health

    public void HitEnemy(float damageAmount, Vector2 hitPoint, HitSfxType hitType, float knockbackForce)
    {
        CollisionManager.TryStagger(damageAmount);
        enemyAnimationManager.SetTriggerForAnimation("Hit");

        PlayBloodEffect();

        if (hitType != HitSfxType.none)
            AudioManager.Instance.PlaySFX(CollisionManager.GetHitSound(hitType), transform.position, 0.4f);

        ApplyDamage(damageAmount * DamageModifier);
        SpawnDamagePopUp(damageAmount * DamageModifier);
        UpdateEnemyHealthBar();
    }

    private void PlayBloodEffect()
    {
       
        GameObject go = Instantiate(enemyConfig.bloofVFXPrefabs[Random.Range(0, enemyConfig.bloofVFXPrefabs.Length)], GetEnemyPos(), Quaternion.identity);
        Vector3 scale = go.transform.localScale;

        int randX = (int)MyUtils.GetRandomValue<int>(new int[] { -1, 1 });
        scale.x *= randX;
        go.transform.localScale = scale;
    }


    //public IEnumerator EnemyHitCoroutine(float damageAmount, Vector2 hitPoint, HitSfxType hitType, float knockbackForce)
    //{     
    //    //material.SetFloat("_Flash", 1);   
    //    yield return new WaitForSeconds(0.1f);
    //    //material.SetFloat("_Flash", 0);
    //}

    public void ApplyDamage(float amount)
    {
        if (isDead) return;

        float damage = amount * DamageModifier;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) Die();
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth = amount;
    }

    public void UpdateEnemyHealthBar()
    {
        Vector3 scale = new(CurrentHealth / MaxHealth, 1, 1);
        healthbarFG.localScale = scale;
    }

    private void SpawnDamagePopUp(float damage)
    {
        var obj = Instantiate(damagePopUp, transform.position + Vector3.up, Quaternion.identity);
        obj.SetText(damage.ToString());
    }

    public void Die()
    {
        CurrentHealth = 0;
        ChangeState(EnemyStateEnum.Death);
    }

    #endregion

    #region Reset / Pooling

    public void ResetEnemy()
    {
        switch (enemyType)
        {
            case EnemyTypeEnum.Arsonist:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(gameObject); break;
            case EnemyTypeEnum.Revenant:
                ObjectPoolManager.Instance.RevenantPool.ReturnToPool(gameObject); break;
            case EnemyTypeEnum.Sinner:
                ObjectPoolManager.Instance.SinnerPool.ReturnToPool(gameObject); break;
            case EnemyTypeEnum.Necromancer:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(gameObject); break;
        }

        IsDead = false;
        CollisionManager.Rb.bodyType = RigidbodyType2D.Dynamic;
        CollisionManager.BoxCollider.enabled = true;
        worldCanvas.gameObject.SetActive(true);

        attackState.IsAttackDelayOver = true;
        enemyAnimationManager.Animator.enabled = true;

        RestoreHealth(MaxHealth);
        UpdateEnemyHealthBar();
        ChangeState(EnemyStateEnum.Idle);
    }

    #endregion

    public Vector3 GetEnemyPos() => transform.position;
}
