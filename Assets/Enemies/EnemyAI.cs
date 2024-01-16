using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Stun,
        Death
    }

    public EnemyState currentState = EnemyState.Idle;

    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    
    public EnemyCollision enemyCollision;
    private EnemyMovement enemyMovement;
    private MinionAttack minionAttack;

    [SerializeField] GameObject enemyStunEffect;
    [SerializeField] private ParticleSystem deathEffectParticle;

    [SerializeField] protected float attackRange;

    private int stunTreshold = 100;
    private int currentStunValue = 0;
    private bool isStuned = false;

    [SerializeField] float stunDuration = 2f;
    public float stunTimer = 0;

    protected GameObject player;
    protected bool isDead = false;

    private Vector3 nextPatrollPosition;
    private Vector3 startPosition;


    public float patrolSpeed = 0.75f;
    public float chaseSpeed = 1.5f;

   


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        minionAttack = GetComponent<MinionAttack>();
        enemyCollision = GetComponent<EnemyCollision>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        


        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Start()
    {
        startPosition = transform.position;
    }

    protected virtual void Update()
    {

        ManageStates();


    }

    protected virtual void ManageStates()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Patrol:
                HandlePatrolState();
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
            case EnemyState.Stun:
                HandleStunState();
                break;
            case EnemyState.Death:
                HandleDeathState();
                break;
        }
    }

    #region "IdleState"
    protected void HandleIdleState()
    {
        StartCoroutine(PauseBeforePatrolling());
    }

    protected IEnumerator PauseBeforePatrolling()
    {
        yield return new WaitForSeconds(0);
        FindNextPatrolPoint();


        currentState = EnemyState.Patrol;
        animator.SetBool("isRunning", true);

    }
    #endregion  


    #region "PatrolState"
    protected void HandlePatrolState()
    {
        if (Vector2.Distance(transform.position, nextPatrollPosition) < 0.25f)
        {
            animator.SetBool("isRunning", false);
            currentState = EnemyState.Idle;

        }
        else
        {
            enemyMovement.MoveTo(transform.position, nextPatrollPosition, patrolSpeed);
        }

    }

    protected void FindNextPatrolPoint()
    {
        int patrolDirection = GetPatrolPointDirection();
        int randomRange = Random.Range(2, 4);
        nextPatrollPosition = new Vector2(startPosition.x + (patrolDirection * randomRange), startPosition.y);
    }

    protected int GetPatrolPointDirection()
    {
        int direction = 0;
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        return direction;

    }

    #endregion


    #region "ChaseState"
    protected void HandleChaseState()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
        {
            currentState = EnemyState.Attack;

        }
        else
        {
            enemyMovement.MoveTo(transform.position, player.transform.position, chaseSpeed);
        }

    }

    public void OnEnterChaseState()
    {
        if (!isDead)
        {
            currentState = EnemyAI.EnemyState.Chase;
            animator.SetBool("isRunning", true);
        }
       
    }

    public void OnExitChaseState()
    {
        currentState = EnemyAI.EnemyState.Idle;
        animator.SetBool("isRunning", false);
    }

    #endregion

    #region "AttackState"
    protected virtual void HandleAttackState()
    {
        if (!isDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isHit", false);
                animator.SetBool("isAttacking", true);
                

            }

            else
            {
                EndAttackAnim();
                currentState = EnemyState.Chase;
                animator.SetBool("isRunning", true);
            }
        }
       
    }

    public void BoxCastForAttack()
    {
        minionAttack.BoxCast();

    }

    public virtual void EndAttackAnim()
    {
        animator.SetBool("isAttacking", false);
    }
    #endregion

    #region "StunState"
    protected virtual void HandleStunState()
    {
       if(stunTimer >= stunDuration)
        {
            
            stunTimer = 0f;
            isStuned = false;
            enemyStunEffect.SetActive(false);
            Debug.Log("StunOver");
            currentState = EnemyAI.EnemyState.Chase;
        }

        else
        {
            stunTimer += Time.deltaTime;
        }
        
    }

    protected virtual void EnterStunState()
    {

        Debug.Log("Stun");
        animator.SetBool("isRunning", false);
        EndAttackAnim();
        currentState = EnemyAI.EnemyState.Stun;
        enemyStunEffect.SetActive(true);
            
    }

    //public void EndHitAnim()
    //{
    //    animator.SetBool("isHit", false);
    //}
    #endregion

    #region "DeathState"
    protected void HandleDeathState()
    {
        

    }

    public void OnEnterDeathtState()
    {
        isDead = true;
        animator.SetBool("isRunning", false);
        EndAttackAnim();
        //animator.SetBool("isHit", false);
        currentState = EnemyAI.EnemyState.Death;
        enemyStunEffect.SetActive(false);
        StartCoroutine(OnEnemyDeath());
        //animator.SetTrigger("Death");
    }

    private IEnumerator  OnEnemyDeath()
    {
        //Destroy(this.gameObject);
        animator.enabled = false;
        spriteRenderer.enabled = false;
        deathEffectParticle.Play();
        yield return new WaitForSeconds(0.55f);
        Destroy(this.gameObject);
        
    }
    #endregion


    public void ManageStunValue(int damage)
    {
        int value = Random.Range(damage - damage/2, damage +1);
        currentStunValue += 50 ;
        if (currentStunValue >= stunTreshold)
        {
            isStuned = true;
            currentStunValue = 0;
            EnterStunState();
        }       
    }

   
}
