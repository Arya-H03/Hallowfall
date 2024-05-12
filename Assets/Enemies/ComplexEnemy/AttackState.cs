using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    
    public float swordAttackTimer = 0f;
    private float swordAttackCooldown = 2f;

    private bool canCancelSwordAttack = true;    
    private bool isAttacking = false;

    private float attackCancelingChance = 0.33f;    

    private SwordAttack swordAttack;

    public AttackState() : base()
    {
        stateEnum = EnemyStateEnum.Attack;

    }

    private void Awake()
    {
        swordAttack = GetComponentInChildren<SwordAttack>();
    }

    public override void OnEnterState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
    }

    public override void OnExitState()
    {
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
    }

    public override void HandleState()
    {
        
        if(statesManager.player.GetComponent<PlayerController>().isParrying && canCancelSwordAttack &&isAttacking)
        {
            canCancelSwordAttack = false;
            int randomNumber = Random.Range(1, 101);
            if (randomNumber <= 100 * attackCancelingChance)
            {              
                CancelSwordAttack();
            }
            
        }
        if (swordAttackTimer >= swordAttackCooldown)
        {
            BeginSwordAttack();
        }

        if (Vector2.Distance(statesManager.player.transform.position, gameObject.transform.position) >= 2)
        {
            CancelSwordAttack();
            statesManager.ChangeState(EnemyStateEnum.Chase);
        }
     // swordAttack.DrawCast();
    }

    private void BeginSwordAttack()
    {
        if(!isAttacking)
        {
            statesManager.animationManager.SetBoolForAnimation("isAttackingSword", true);
            canCancelSwordAttack = true;
            isAttacking = true;
        }
        
    }

    public void EndSwordAttack()
    {
        if (isAttacking)
        {
            statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
            swordAttackTimer = 0f;
            isAttacking = false;
        }
       
    }

    public void CancelSwordAttack()
    {
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        isAttacking = false;
        swordAttackTimer = 0f;
    }



    public void ManageSwordAttackCooldown()
    {
        if(swordAttackTimer < swordAttackCooldown)
        {
            swordAttackTimer += Time.deltaTime;
        }
        
    }

    public void EnableBoxCastingForSwordAttack()
    {
        
        swordAttack.SwordAttackBoxCast();
    }



}
