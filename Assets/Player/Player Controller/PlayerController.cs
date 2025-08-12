using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CCoroutineRunner))]
public class PlayerController : MonoBehaviour
{
    #region === Component References ===
    private PlayerAnimationHandler playerAnimationHandler;
    private PlayerMovementHandler playerMovementHandler;
    private AfterImageHandler afterImageHandler;
    private PlayerPhysicsHandler playerPhysicsController;
    private PlayerAbilityHandler playerAbilityController;
    private PlayerVFXHandler playerVFXHandler;
    private PlayerSFXHandler playerSFXHandler;
    private PlayerEnvironmentCheckHandler playerEnvironmentChecker;
    private PlayerHitHandler playerHitHandler;
    private PlayerHealthBarHandler playerHealthBarHandler;
    private PlayerInputHandler playerInputHandler;
    private PlayerSignalHub signalHub;
    private EnemyDetector enemyDetector;
    private CCoroutineRunner coroutineRunner;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region === Serialized Config & Objects ===
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private DashAttackBox dashAttackBox;
    [SerializeField] private PlayerParryShield parryShield;
    [SerializeField] private string enemyTag;
    #endregion

    #region === Runtime Data ===
    private float maxHealth;
    private float currentHealth;
    private int currentEssence;
    private int atonementLvl;
    private int atonementToLevel;
    #endregion

    #region === Player State Flags ===
    [SerializeField] private bool isParrying = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isRolling = false;
    [SerializeField] private bool canRoll = true;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool isImmune = false;
    [SerializeField] private FloorTypeEnum currentFloorType;
    #endregion

    #region === State Machine ===
    private PlayerStateMachine stateMachine;
    #endregion

    #region === Properties ===
    public PlayerAnimationHandler PlayerAnimationHandler => playerAnimationHandler;
    public PlayerMovementHandler PlayerMovementHandler => playerMovementHandler;
    public AfterImageHandler AfterImageHandler => afterImageHandler;
    public PlayerPhysicsHandler PlayerPhysicsController => playerPhysicsController;
    public PlayerHealthBarHandler PlayerHealthBarHandler => playerHealthBarHandler;
    public PlayerAbilityHandler PlayerAbilityController => playerAbilityController;
    public PlayerVFXHandler PlayerVFXHandler => playerVFXHandler;
    public PlayerSFXHandler PlayerSFXHandler => playerSFXHandler;
    public PlayerEnvironmentCheckHandler PlayerEnvironmentChecker => playerEnvironmentChecker;
    public PlayerHitHandler PlayerHitHandler => playerHitHandler;
    public PlayerInputHandler PlayerInputHandler => playerInputHandler;
    public PlayerSignalHub PlayerSignalHub => signalHub;
    public EnemyDetector EnemyDetector => enemyDetector;
    public CCoroutineRunner CoroutineRunner => coroutineRunner;
    public Rigidbody2D Rb => rb;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public PlayerConfig PlayerConfig => playerConfig;
    public PlayerStateMachine StateMachine => stateMachine;
    public DashAttackBox DashAttackBox => dashAttackBox;
    public PlayerParryShield ParryShield => parryShield;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentEssence { get => currentEssence; set => currentEssence = value; }
    public int AtonementLvl { get => atonementLvl; set => atonementLvl = value; }
    public int AtonementToLevel { get => atonementToLevel; set => atonementToLevel = value; }
    public bool CanDash { get => stateMachine.PlayerDashState.CanDashAttack();}
    public bool IsParrying { get => isParrying; set => isParrying = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsRolling { get => isRolling; set => isRolling = value; }
    public bool CanRoll { get => canRoll; set => canRoll = value; }
    public bool IsImmune { get => isImmune; set => isImmune = value; }

    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }
    public string EnemyTag { get => enemyTag; set => enemyTag = value; }
    
    #endregion

    #region === Unity Callbacks ===
    private void Awake()
    {
       
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        coroutineRunner = GetComponent<CCoroutineRunner>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        afterImageHandler = GetComponentInChildren<AfterImageHandler>();
        playerAnimationHandler = GetComponentInChildren<PlayerAnimationHandler>();
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        playerPhysicsController = GetComponent<PlayerPhysicsHandler>();
        playerAbilityController = GetComponentInChildren<PlayerAbilityHandler>();
        playerEnvironmentChecker = GetComponentInChildren<PlayerEnvironmentCheckHandler>();
        playerHealthBarHandler = GetComponent<PlayerHealthBarHandler>();
        enemyDetector = GetComponentInChildren<EnemyDetector>();
        playerHitHandler = GetComponent<PlayerHitHandler>();
        playerSFXHandler = GetComponent<PlayerSFXHandler>();
        playerVFXHandler = GetComponent<PlayerVFXHandler>();

        signalHub = new PlayerSignalHub();
    }

    private void Start()
    {
        InitStats();
        InjectDependencies();

        stateMachine = new PlayerStateMachine(this);
        stateMachine.InitAllStates();

        GameManager.Instance.InitSkillsFromSkillTree();
    }

    private void Update()
    {
        stateMachine.CurrentState?.FrameUpdate();
        ManageTimers();
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState?.PhysicsUpdate();
    }
    #endregion

    #region === Initialization ===
    private void InjectDependencies()
    {
        playerInputHandler.Init(this);
        playerMovementHandler.Init(this);
        playerHealthBarHandler.Init(this);
        playerHitHandler.Init(this);
        playerAnimationHandler.Init(this);
        playerSFXHandler.Init(this);
        playerVFXHandler.Init(this);
    }

    private void InitStats()
    {
        maxHealth = playerConfig.maxHealth;
        atonementToLevel = playerConfig.toLevel;
    }
    #endregion

    #region === Input Callbacks ===
    public void OnMoveInput(Vector2 dir)
    {
        playerMovementHandler.SetMovementInputDirection(dir);
    }

    public void OnSwordAttackInput()
    {
        signalHub.OnChangeState?.Invoke(PlayerStateEnum.SwordAttack);
        stateMachine.PlayerSwordAttackState.TrySwordAttack();
    }

    public void OnDashInput()
    {
        if (CanDash)
        {
            signalHub.OnChangeState?.Invoke(PlayerStateEnum.Dash);
        }
    }

    public void OnParryInput()
    {
        if (!isRolling && !isParrying)
        {
            signalHub.OnChangeState?.Invoke(PlayerStateEnum.Parry);
        }
    }

    public void OnRollInput()
    {
        if (!isRolling && canRoll)
        {
            signalHub.OnChangeState?.Invoke(PlayerStateEnum.Roll);
        }
    }
    #endregion

    #region === Utilities ===
    public void ResetPlayerVariables()
    {
        isParrying = false;
        isDead = false;
        isAttacking = false;
        isRolling = false;
        canRoll = true;
    }

    public Vector3 GetPlayerPos() => transform.position;

    private void ManageTimers()
    {
        stateMachine.PlayerDashState.HandleDashTimer();
    }
    #endregion
}
