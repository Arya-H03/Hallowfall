using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnightAI : EnemyAI
{
    private KnightAttack knightAttack;

    
    

    [SerializeField] float lightAttackCD = 1f;
    private float lightAttackTimer;

    [SerializeField] float heavyAttackCD = 3f;
    private float heavyAttackTimer;

    private bool isAttacking = false;
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        knightAttack = GetComponent<KnightAttack>();
       
        lightAttackTimer = lightAttackCD;
        heavyAttackTimer = heavyAttackCD;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (lightAttackTimer < lightAttackCD)
        {
            lightAttackTimer += Time.deltaTime;
        }

        if (heavyAttackTimer < heavyAttackCD)
        {
            heavyAttackTimer += Time.deltaTime;
        }
    }

    protected override void HandleAttackState()
    {
        if (!isDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
            {
                animator.SetBool("isRunning", false);
           
                
                HandelLightAttack();
                HandelHeavyAttack();

            }

            else
            {
                currentState = EnemyState.Chase;
                animator.SetBool("isRunning", true);
            }
        }



    }

    #region AttackState
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

    }

    public void BoxCastForHeavyAttack()
    {
        knightAttack.HeavyAttackBoxCast();

    }

    public override void EndAttackAnim()
    {
        animator.SetBool("isLightAttacking", false);
        animator.SetBool("isHeavyAttacking", false);
        isAttacking = false;
    }

    #endregion

    #region HitState


    #endregion

}
