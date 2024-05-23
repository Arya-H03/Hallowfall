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
        Death,
        Dash
    }

    public EnemyState currentState = EnemyState.Idle;

    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    
    public EnemyCollision enemyCollision;
    protected EnemyMovement enemyMovement;
   
    [SerializeField] GameObject enemyStunEffect;

    [SerializeField] GameObject essence;
    [SerializeField] int numberOfEssence = 2;

    [SerializeField] protected ParticleSystem deathEffectParticle;

    [SerializeField] protected float attackRange;

    private int stunTreshold = 100;
    public int currentStunValue = 0;
    protected bool isStuned = false;
    private bool isStunedRecently = false;

    [SerializeField] float stunDuration = 2f;
    public float stunTimer = 0;

    protected GameObject player;
    protected bool isDead = false;

    private Vector3 nextPatrollPosition;
    private Vector3 startPosition;


    public float patrolSpeed = 0.75f;
    public float chaseSpeed = 1.5f;

    protected bool canAttack = true;
    [SerializeField] protected float attackDelay = 0.75f;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
        enemyCollision = GetComponent<EnemyCollision>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Start()
    {
        startPosition = transform.position;
        StartCoroutine(DecreaseStunValueOverTime());
    }

    protected virtual void Update()
    {
        ManageStates();
    }

    public void ChangeState(EnemyState state)
    {
        //Debug.Log(currentState.ToString() + " to " + state.ToString());
        currentState = state;       
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

    private IEnumerator PauseBeforePatrolling()
    {
        yield return new WaitForSeconds(0);
        FindNextPatrolPoint();

        ChangeState(EnemyState.Patrol);
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

    private void FindNextPatrolPoint()
    {
        int patrolDirection = GetPatrolPointDirection();
        int randomRange = Random.Range(2, 4);
        nextPatrollPosition = new Vector2(startPosition.x + (patrolDirection * randomRange), startPosition.y);
    }

    private int GetPatrolPointDirection()
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
    protected virtual void HandleChaseState()
    {
        if(!player.GetComponent<PlayerController>().IsDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
            {

                ChangeState(EnemyState.Attack);
            }
            else
            {
                enemyMovement.MoveTo(transform.position, player.transform.position, chaseSpeed);
            }

        }
        else
        {
            OnExitChaseState();
        }

    }

    public void OnEnterChaseState()
    {
        if (!isDead && !isStuned)
        {
            ChangeState(EnemyState.Chase);
            animator.SetBool("isRunning", true);
        }
       
    }

    public void OnExitChaseState()
    {
        ChangeState(EnemyState.Idle);
        animator.SetBool("isRunning", false);
    }

    #endregion

    #region "AttackState"
    protected virtual void HandleAttackState()
    {
        
    }

    public virtual void EndAttackAnim()
    {
       
    }

    protected IEnumerator ManageAttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    #endregion

    #region "StunState"
    protected virtual void HandleStunState()
    {
       if(stunTimer >= stunDuration)
        {

            OnExitStunState();
        }

        else
        {
            stunTimer += Time.deltaTime;
        }
        
    }

    protected virtual void OnEnterStunState()
    {

        
        animator.SetBool("isRunning", false);
        EndAttackAnim();
        ChangeState(EnemyState.Stun);
        Debug.Log("Stun");
        enemyStunEffect.SetActive(true);
            
    }

    protected virtual void OnExitStunState()
    {

        stunTimer = 0f;
        enemyStunEffect.SetActive(false);      
        isStuned = false;
        Debug.Log("StunOver");
        OnEnterChaseState();

    }

    public void ManageStunValue(int damage)
    {
        if (!isStuned && !isStunedRecently)
        {
            int value = Random.Range(damage - damage / 2, damage + 1);
            currentStunValue += 50;
            if (currentStunValue >= stunTreshold)
            {               
                StartCoroutine(ManageBeingStun());            
            }
        }
        
    }

    private IEnumerator ManageBeingStun()
    {
        isStuned = true;
        isStunedRecently = true;
        currentStunValue = 0;
        OnEnterStunState();
        yield return new WaitForSeconds(stunDuration + 5f);
        isStunedRecently = false;
    }
    private IEnumerator DecreaseStunValueOverTime() { 

        while(currentStunValue > 0)
        {
            currentStunValue -= 5;
            yield return new WaitForSeconds(0.5f);
        }
        
    }
    #endregion

    #region "DeathState"
    protected virtual void HandleDeathState()
    {
        

    }

    public void OnEnterDeathtState()
    {
        isDead = true;
        animator.SetBool("isRunning", false);
        EndAttackAnim();
        ChangeState(EnemyState.Death);
        enemyStunEffect.SetActive(false);
        StartCoroutine(OnEnemyDeath());
    }

    private IEnumerator  OnEnemyDeath()
    {
        
        animator.enabled = false;
        spriteRenderer.enabled = false;
        deathEffectParticle.Play();
        for(int i =0; i< numberOfEssence; i++)
        {
            Instantiate(essence, transform.position, Quaternion.identity);
        }  
        
        yield return new WaitForSeconds(0.55f);
        Destroy(this.gameObject);
        
    }
    #endregion


   

   
}
