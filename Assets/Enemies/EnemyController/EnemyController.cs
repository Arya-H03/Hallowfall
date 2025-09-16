using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CCoroutineRunner))]
[RequireComponent(typeof(CDetector))]
public class EnemyController : MonoBehaviour
{
   
    [Header("Data")]
    [SerializeField] protected EnemyConfigSO enemyConfig;
    [SerializeField] protected EnemyTypeEnum enemyType;
    [SerializeField] protected List<EnemyBehaviorSO> listOfBehaviorSO;
    private List<EnemyBehaviorSO> listOfBehaviors;
    [SerializeField] bool canFlashOnHit = false;

    [Header("Prefabs")]
    [SerializeField] public GameObject stunEffect;

   
    #region Components

    protected EnemySignalHub signalHub;
    protected BoxCollider2D boxCollider;
    protected CCoroutineRunner coroutineRunner;
    protected EnemyAnimationHandler enemyAnimationHandler;
    protected EnemyEnvironenmentCheckHandler enemyEnvironenmentCheckHandler;
    protected EnemyHealthbarHandler enemyHealthbarHandler;
    protected EnemyHitHandler enemyHitHandler;
    protected EnemyItemDropHandler enemyItemDropHandler;
    protected EnemyMovementHandler enemyMovementHandler;
    protected EnemyPhysicsHandler enemyPhysicsHandler;
    protected EnemySFXHandler enemySFXHandler;
    protected EnemyVFXHandler enemyVFXHandler;
    protected Material material;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected CDetector detector;

    protected EnemyStateMachine stateMachine;

    #endregion

    #region Runtime References

    private GameObject playerGO;
    private PlayerController playerController;
    private List<EnemyBaseAttack> attacks;

    #endregion

    #region Flags

    private bool hasSeenPlayer = false;
    private bool canAttack = true;
    private bool isAttacking = false;
    private bool isAttackDelayOver = true;
    private bool isStuned = false;
    private bool isPlayerDead = false;
    private bool isBeingknocked = false;

    private bool canMove = true;
    private bool isDead = false;

    #endregion

    #region Stats

    private int enemyLvl;
    private FloorTypeEnum currentFloorType;

    #endregion

    #region Properties

    public EnemyTypeEnum EnemyType => enemyType;
    public FloorTypeEnum CurrentFloorType { get => currentFloorType; set => currentFloorType = value; }

    public EnemySignalHub SignalHub { get => signalHub; }
    public BoxCollider2D BoxCollider => boxCollider;
    public CCoroutineRunner CoroutineRunner => coroutineRunner;
    public EnemyAnimationHandler EnemyAnimationHandler => enemyAnimationHandler;
    public EnemySFXHandler EnemySFXHandler { get => enemySFXHandler; }
    public EnemyConfigSO EnemyConfig => enemyConfig;
    public EnemyEnvironenmentCheckHandler EnemyEnvironenmentCheckHandler => enemyEnvironenmentCheckHandler;
    public EnemyHealthbarHandler EnemyHealthbarHandler => enemyHealthbarHandler;
    public EnemyHitHandler EnemyHitHandler => enemyHitHandler;
    public EnemyItemDropHandler EnemyItemDropHandler => enemyItemDropHandler;    
    public EnemyMovementHandler EnemyMovementHandler => enemyMovementHandler;
    public EnemyPhysicsHandler EnemyPhysicsHandler => enemyPhysicsHandler;
    public EnemyVFXHandler EnemyVFXHandler { get => enemyVFXHandler; }
    public EnemyStateMachine EnemyStateMachine => stateMachine; 
    public Material Material { get => material; set => material = value; }
    public PlayerController PlayerController => playerController;
    public GameObject PlayerGO { get => playerGO; set => playerGO = value; }
    public Rigidbody2D Rb => rb;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public CDetector Detector { get => detector; }
    public List<EnemyBaseAttack> Attacks => attacks;
    public List<EnemyBehaviorSO> ListOfBehaviors { get => listOfBehaviors; }

    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsBeingknocked { get => isBeingknocked; set => isBeingknocked = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsPlayerDead { get => isPlayerDead; set => isPlayerDead = value; }
    public int EnemyLvl { get => enemyLvl; set => enemyLvl = value; }
    public bool HasSeenPlayer { get => hasSeenPlayer; set => hasSeenPlayer = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool IsStuned { get => isStuned; set => isStuned = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public bool IsAttackDelayOver { get => isAttackDelayOver; set => isAttackDelayOver = value; }
    public bool CanFlashOnHit { get => canFlashOnHit;}
    




    #endregion


    private void Awake()
    {
        coroutineRunner = GetComponent<CCoroutineRunner>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        material = spriteRenderer.material;
        detector = GetComponent<CDetector>();

        enemyAnimationHandler = GetComponentInChildren<EnemyAnimationHandler>();
        enemyMovementHandler = GetComponent<EnemyMovementHandler>();
        enemyPhysicsHandler = GetComponentInChildren<EnemyPhysicsHandler>();
        enemyItemDropHandler = GetComponentInChildren<EnemyItemDropHandler>();
        enemyEnvironenmentCheckHandler = GetComponentInChildren<EnemyEnvironenmentCheckHandler>();
        enemyHitHandler = GetComponent<EnemyHitHandler>();
        enemyHealthbarHandler = GetComponentInChildren<EnemyHealthbarHandler>();
        enemySFXHandler = GetComponent<EnemySFXHandler>();
        enemyVFXHandler = GetComponent<EnemyVFXHandler>();

        signalHub = new EnemySignalHub();
       
    }

    private void Start()
    {
        InitializeEnemyStats();

        playerGO = GameManager.Instance.Player;
        playerController = GameManager.Instance.PlayerController;

        stateMachine = new EnemyStateMachine(this);

        //Call before Initialzing enemy states
        InjectDependencies();

        //Call after Dependency Injection
        stateMachine.InitAllStates();

        listOfBehaviors = new List<EnemyBehaviorSO>();
        foreach (var behavior in listOfBehaviorSO)
        {
            var copy = Instantiate(behavior);
            listOfBehaviors.Add(copy);
            copy.InitBehavior(this);
        }

        signalHub.OnInitWithEnemyController?.Invoke(this);

        stateMachine.ChangeState(EnemyStateEnum.Chase); 
    }
    private void InitializeEnemyStats()
    {
        hasSeenPlayer = true;
        enemyLvl = 1;
    }
    private void InjectDependencies()
    {
        enemyHealthbarHandler.Init(this);
        enemyAnimationHandler.Init(this);
        enemyPhysicsHandler.Init(this);
        enemyMovementHandler.Init(this);
        enemyHitHandler.Init(this);
       
        enemySFXHandler.Init(this);
        enemyVFXHandler.Init(this);
        enemyItemDropHandler.Init(this);
    }
    private void Update()
    {
        if (isDead) return;
        stateMachine.CurrentState?.FrameUpdate();
        
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState?.PhysicsUpdate();
    }

    public List<BaseEnemyAbilitySO> GetListOfAllAbilities()
    {
        List<BaseEnemyAbilitySO> abilityList = new List<BaseEnemyAbilitySO>();
        foreach (BaseEnemyAbilitySO ability in enemyConfig.abilitylist)
        {
            abilityList.Add((Instantiate(ability)));
        }

        return abilityList;
    }

    public Vector3 GetEnemyPos() => transform.position;

}
