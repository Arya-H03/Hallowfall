using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnightAI : EnemyAI
{
    private KnightAttack knightAttack;

    [SerializeField] float lightAttackCD = 0.5f;
    private float lightAttackTimer;

    [SerializeField] float heavyAttackCD = 2f;
    private float heavyAttackTimer;

    [SerializeField] float DashCD = 5f;
    private float dashTimer;
    private float dashSpeed = 8f;
    private bool isDashing = false;

    private bool isAttacking = false;
    protected override void Awake()
    {
        base.Awake();
        knightAttack = GetComponent<KnightAttack>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
             
        lightAttackTimer = lightAttackCD;
        heavyAttackTimer = heavyAttackCD;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


        if (dashTimer < DashCD)
        {
            dashTimer += Time.deltaTime;
        }

        if (lightAttackTimer < lightAttackCD)
        {
            lightAttackTimer += Time.deltaTime;
        }

        if (heavyAttackTimer < heavyAttackCD)
        {
            heavyAttackTimer += Time.deltaTime;
        }

        
        
    }

    protected override void ManageStates()
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
            case EnemyState.Dash:
                HandleDashState();
                break;
        }
    }

    protected override void HandleChaseState()
    {
        if (!player.GetComponent<PlayerController>().IsDead)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < 5 && distanceToPlayer > 2.5f && dashTimer >= DashCD && !isAttacking && !isStuned)
            {
                OnEnterDashState();
            }
            else if (Vector2.Distance(transform.position, player.transform.position) < attackRange && !isDashing && !isStuned && canAttack)
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

    #region AttackState

    protected override void HandleAttackState()
    {
        if (!isDead &&!player.GetComponent<PlayerController>().IsDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < attackRange && canAttack )
            {
                animator.SetBool("isRunning", false);


                HandelLightAttack();
                HandelHeavyAttack();

            }

            else
            {
                OnEnterChaseState();
                animator.SetBool("isRunning", true);
            }
        }
        else
        {
            ChangeState(EnemyState.Patrol);
        }



    }
    private void HandelLightAttack()
    {
        if(!isAttacking && lightAttackTimer >= lightAttackCD)
        {
            isAttacking = true; 
            animator.SetBool("isLightAttacking", true);
            lightAttackTimer = 0f;
        }
        
    }

    private void HandelHeavyAttack()
    {
        if (!isAttacking && heavyAttackTimer >= heavyAttackCD)
        {
            isAttacking = true;
            animator.SetBool("isHeavyAttacking", true);
            heavyAttackTimer = 0f;
        }

    }
    public void BoxCastForLightAttack()
    {
        knightAttack.LightAttackBoxCast();
        StartCoroutine(ManageAttackDelay());
    }

    public void BoxCastForHeavyAttack()
    {
        knightAttack.HeavyAttackBoxCast();
        StartCoroutine(ManageAttackDelay());
    }

    public override void EndAttackAnim()
    {
        animator.SetBool("isLightAttacking", false);
        animator.SetBool("isHeavyAttacking", false);
        isAttacking = false;
    }

    #endregion

    #region DashState
    private void HandleDashState()
    {

    }

    private void OnEnterDashState()
    {
        if(dashTimer >= DashCD)
        {
            ChangeState(EnemyState.Dash);
            EndAttackAnim();
            animator.SetBool("isRunning", false);
            isDashing = true;
            enemyCollision.isInvincible = true;
            StartDash();
            dashTimer = 0f;
        }        

    }

    private void OnExitDashState()
    {
        animator.SetBool("isDashing", false);
        isDashing = false;
        enemyCollision.isInvincible = false;
        OnEnterChaseState();
    }

    private void StartDash()
    {
        Vector2 direction = new Vector2(player.transform.position.x - this.transform.position.x , player.transform.position.y - this.transform.position.y).normalized;
        animator.SetBool("isDashing", true);
        rb.velocity += direction * dashSpeed;
        

    }

    public void EndDash()
    {
        OnExitDashState();

    }
    #endregion

}
