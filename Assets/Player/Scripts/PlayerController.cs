using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    private PlayerAnimationController animationController;
    private PlayerMovementHandler playerMovementHandler;
    private AfterImageHandler afterImageHandler;
    private PlayerPhysicsController playerPhysicsController;
    private PlayerAbilityController playerAbilityController;
    private PlayerEnvironmentChecker playerEnvironmentChecker;  
    private Material material;
    private SpriteRenderer spriteRenderer;
    private EnemyDetector enemyDetector;

    // Config
    [SerializeField] private PlayerConfig playerConfig;


    [SerializeField] private Transform playerCenterTransform;
    [SerializeField] private string enemyTag;


    // PlayerGO Stats
    private float maxHealth;
    private float currentHealth;
    private int currentEssence;
    private int atonementLvl;
    private int atonementToLevel;

    // State Flags
    [SerializeField] private bool canPlayerJump = true;
    [SerializeField] private bool isPlayerGrounded = true;
    [SerializeField] private bool isPlayerJumping = false;
    [SerializeField] private bool canPlayerAttack = true;
    [SerializeField] private bool isParrying = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isRolling = false;
    [SerializeField] private bool isHanging = false;
    [SerializeField] private bool canRoll = true;
    [SerializeField] private bool canHang = true;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool isFacingWall = false;
    [SerializeField] private bool isFacingLedge = false;
    [SerializeField] private bool isImmune = false;

    // FSM
    [SerializeField] private PlayerStateEnum currentStateEnum;
    private PlayerBaseState currentState;
    [SerializeField] private FloorTypeEnum currentFloorType;

    // States
    private PlayerIdleState playerIdleState;
    private PlayerRunState playerRunState;
    private PlayerSwordAttackState playerSwordAttackState;
    private PlayerParryState playerParryState;
    private PlayerRollState playerRollState;
    private PlayerDeathState playerDeathState;
    private PlayerDashState playerDashState;

    #region Getters / Setters
    public PlayerAnimationController AnimationController { get => animationController; set => animationController = value; }
    public PlayerMovementHandler PlayerMovementHandler { get => playerMovementHandler; set => playerMovementHandler = value; }
    public AfterImageHandler AfterImageHandler { get => afterImageHandler; set => afterImageHandler = value; }
    public PlayerPhysicsController PlayerPhysicsController { get => playerPhysicsController; set => playerPhysicsController = value; }
    public PlayerAbilityController PlayerAbilityController { get => playerAbilityController; }
    public Material Material { get => material; set => material = value; }
    public PlayerConfig PlayerConfig { get => playerConfig; set => playerConfig = value; }

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentEssence { get => currentEssence; set => currentEssence = value; }
    public int AtonementLvl { get => atonementLvl; set => atonementLvl = value; }
    public int AtonementToLevel { get => atonementToLevel; set => atonementToLevel = value; }

    public bool CanPlayerJump { get => canPlayerJump; set => canPlayerJump = value; }
    public bool IsPlayerGrounded { get => isPlayerGrounded; set => isPlayerGrounded = value; }
    public bool IsPlayerJumping { get => isPlayerJumping; set => isPlayerJumping = value; }
    public bool CanPlayerAttack { get => canPlayerAttack; set => canPlayerAttack = value; }
    public bool IsParrying { get => isParrying; set => isParrying = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsRolling { get => isRolling; set => isRolling = value; }
    public bool CanRoll { get => canRoll; set => canRoll = value; }
    public bool IsHanging { get => isHanging; set => isHanging = value; }
    public bool CanHang { get => canHang; set => canHang = value; }
    public bool IsFalling { get => isFalling; set => isFalling = value; }
    public bool IsFacingWall { get => isFacingWall; set => isFacingWall = value; }
    public bool IsFacingLedge { get => isFacingLedge; set => isFacingLedge = value; }
    public bool IsImmune { get => isImmune; set => isImmune = value; }

    public PlayerStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }

    public PlayerIdleState PlayerIdleState { get => playerIdleState; set => playerIdleState = value; }
    public PlayerRunState PlayerRunState { get => playerRunState; set => playerRunState = value; }
    public PlayerSwordAttackState PlayerSwordAttackState { get => playerSwordAttackState; set => playerSwordAttackState = value; }
    public PlayerParryState PlayerParryState { get => playerParryState; set => playerParryState = value; }
    public PlayerRollState PlayerRollState { get => playerRollState; set => playerRollState = value; }
    public PlayerDeathState PlayerDeathState { get => playerDeathState; set => playerDeathState = value; }
    public PlayerDashState PlayerDashState { get => playerDashState; set => playerDashState = value; }
    public PlayerEnvironmentChecker PlayerEnvironmentChecker { get => playerEnvironmentChecker; set => playerEnvironmentChecker = value; }
    public string EnemyTag { get => enemyTag; set => enemyTag = value; }
    public EnemyDetector EnemyDetector { get => enemyDetector;}

    #endregion

    private void Awake()
    {
       
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        material = spriteRenderer.material;

        AfterImageHandler = GetComponentInChildren<AfterImageHandler>();
        AnimationController = GetComponentInChildren<PlayerAnimationController>();
        PlayerMovementHandler = GetComponent<PlayerMovementHandler>();
        PlayerPhysicsController = GetComponentInChildren<PlayerPhysicsController>();
        playerAbilityController = GetComponentInChildren<PlayerAbilityController>();
        playerEnvironmentChecker = GetComponentInChildren<PlayerEnvironmentChecker>();
        enemyDetector = GetComponentInChildren<EnemyDetector>();

        // State initialization
        InitializeState(ref playerIdleState);
        InitializeState(ref playerRunState);;
        InitializeState(ref playerSwordAttackState);
        InitializeState(ref playerParryState);
        InitializeState(ref playerRollState);
        InitializeState(ref playerDeathState);
        InitializeState(ref playerDashState);

        // Set default state
        ChangeState(PlayerStateEnum.Idle);
    }

    private void Start()
    {
        InitVariablesFromConfig();
        RestoreHealth(maxHealth);
        GameManager.Instance.InitSkillsFromSkillTree();
    }

    private void Update()
    {
        currentState?.HandleState();
        ManageTimers();
    }

    private void InitializeState<T>(ref T state) where T : PlayerBaseState
    {
        state = GetComponentInChildren<T>();
        state.SetOnInitializeVariables(this);
    }

    public void ChangeState(PlayerStateEnum stateEnum)
    {
        if (currentStateEnum == stateEnum) return;

        CurrentState?.OnExitState();

        currentStateEnum = stateEnum;

        CurrentState = stateEnum switch
        {
            PlayerStateEnum.Idle => playerIdleState,
            PlayerStateEnum.Run => (!IsAttacking && !IsParrying && !IsPlayerJumping) ? playerRunState : CurrentState,
            PlayerStateEnum.SwordAttack => playerSwordAttackState,
            PlayerStateEnum.Parry => playerParryState,
            PlayerStateEnum.Roll => playerRollState,
            PlayerStateEnum.Dash => playerDashState,
            PlayerStateEnum.Death => playerDeathState,
            _ => playerIdleState
        };

        CurrentState?.OnEnterState();
    }

    private void InitVariablesFromConfig()
    {
        maxHealth = playerConfig.maxHealth;
        atonementToLevel = playerConfig.toLevel;
    }

    public void OnMove(Vector2 dir)
    {
        PlayerMovementHandler.HandleMovement(dir);
    }

    public void OnSwordAttack()
    {
        if (!isPlayerJumping && !isFalling && isPlayerGrounded && !isHanging)
        {
            ChangeState(PlayerStateEnum.SwordAttack);
            playerSwordAttackState.HandleAttack();
        }
    }

    public void OnDashAttack()
    {
        if(playerDashState.CanDashAttack()) ChangeState(PlayerStateEnum.Dash);

    }

    public void OnStartParry()
    {
        if (isPlayerGrounded && !isRolling && !isFalling && !isPlayerJumping)
        {
            ChangeState(PlayerStateEnum.Parry);
        }
    }

    public void OnRoll()
    {
        if (isPlayerGrounded && !isPlayerJumping && !isRolling && canRoll)
        {
            ChangeState(PlayerStateEnum.Roll);
        }
    }

    public void RestoreHealth(float amount)
    {
        
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void OnTakingDamage(float value)
    {
        if (currentHealth <= 0) return;

        currentHealth -= value;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ChangeState(PlayerStateEnum.Death);
        }
    }

    private IEnumerator PlayerHitCoroutine(float damage)
    {
        if (isImmune) yield break;
        material.SetFloat("_Flash", 1);
        OnTakingDamage(damage);     
        yield return new WaitForSeconds(0.1f);
        material.SetFloat("_Flash", 0);
    }

    public void OnPlayerHit(float damage)
    {
        StartCoroutine(PlayerHitCoroutine(damage));
    }

    public void ResetPlayerVariables()
    {
        canPlayerJump = true;
        isPlayerGrounded = true;
        isPlayerJumping = false;
        canPlayerAttack = true;
        isParrying = false;
        isDead = false;
        isAttacking = false;
        isRolling = false;
        isHanging = false;
        canRoll = true;
        canHang = true;
        isFalling = false;
        isFacingWall = false;
        isFacingLedge = false;
    }

    public Vector3 GetPlayerPos()
    {
        return transform.position;
    }

    private void ManageTimers()
    {
        PlayerDashState.HandleDashTimer();
    }
}
