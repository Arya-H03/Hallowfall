using Google.Protobuf.WellKnownTypes;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class CombatState : EnemyBaseState
{
    public enum CombatActionEnum
    {
        None,
        Move,
        SwordAttack,
        CancelAttack,
        Block
    }

    public void ChangeAction(CombatActionEnum actionEnum)
    {
        if (currentActionEnum != actionEnum && !isAttacking)
        {
            Debug.Log(currentActionEnum + " to " + actionEnum.ToString());

            switch (currentActionEnum)
            {

                case CombatActionEnum.Move:
                    OnExitMoveAction();
                    break;
                case CombatActionEnum.SwordAttack:
                    OnExitSwordAttackAction();
                    break;

            }

            currentActionEnum = actionEnum;

            switch (currentActionEnum)
            {

                case CombatActionEnum.Move:
                    OnEnterMoveAction();
                    break;
                case CombatActionEnum.SwordAttack:
                    OnEnterSwordAttackAction();
                    break;

            }
        }

    }

    [SerializeField] private CombatActionEnum currentActionEnum;

    public float swordAttackTimer = 0f;
    private float swordAttackCooldown = 2f;
    [SerializeField]  private bool  canSwordAttack = true;
    [SerializeField]  private bool isInAttackRange = false;   

    private bool canCancelSwordAttack = true;
    [SerializeField]  private bool isAttacking = false;

    private float attackCancelingChance = 0.33f;    

    private SwordAttack swordAttack;

    private float moveSpeed = 2.5f;

    public CombatState() : base()
    {
        stateEnum = EnemyStateEnum.Combat;

    }

    private void Awake()
    {
        swordAttack = GetComponentInChildren<SwordAttack>();
    }

    public override void OnEnterState()
    {
       
    }

    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {

        //if (enemyController.player.GetComponent<PlayerController>().IsAttacking && enemyController.GetCanBlock())
        //{
        //    ChangeAction(CombatActionEnum.Block);
        //}
        switch (currentActionEnum)
        {
            case CombatActionEnum.Move:
                if (Vector2.Distance(enemyController.player.transform.position, this.transform.position) < 2f)
                {
                    isInAttackRange = true;
                    enemyController.canAttack = true;
                    ChangeAction(CombatActionEnum.SwordAttack);

                }

                else
                {
                    enemyController.canAttack = false;
                }
                enemyController.enemyMovement.MoveTo(transform.position, enemyController.player.transform.position, moveSpeed);
                break;
            case CombatActionEnum.SwordAttack:
                if (canSwordAttack)
                {
                    BeginSwordAttack();
                }

                break;
            case CombatActionEnum.Block:
                enemyController.ChangeState(EnemyStateEnum.Block);
                break;


        }


        if (Vector2.Distance(enemyController.player.transform.position, gameObject.transform.position) >= 2)
        {
            CancelSwordAttack();
            ChangeAction(CombatActionEnum.Move);
            isInAttackRange = false;
        }
        //swordAttack.DrawCast();
    }

    private void BeginSwordAttack()
    {
        if(!isAttacking)
        {
            Debug.Log("Attack");
            canSwordAttack = false;
            enemyController.animationManager.SetBoolForAnimation("isAttackingSword", true);
            canCancelSwordAttack = true;
            isAttacking = true;
            
        }
        
    }

    public void EndSwordAttack()
    {
        if (isAttacking)
        {
            enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);
            swordAttackTimer = 0f;
            isAttacking = false;
        }
       
    }

    public void CancelSwordAttack()
    {
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);
        isAttacking = false;
        swordAttackTimer = 0f;
    }



    public void ManageSwordAttackCooldown()
    {
        if(swordAttackTimer < swordAttackCooldown)
        {
            swordAttackTimer += Time.deltaTime;
            if(swordAttackTimer >= swordAttackCooldown)
            {
                canSwordAttack = true;
            }
        }
        
    }

    public void EnableBoxCastingForSwordAttack()
    {
        
        swordAttack.SwordAttackBoxCast();
    }

    private void OnExitMoveAction()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
    }
    private void OnEnterMoveAction()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", true);
    }

    private void OnExitSwordAttackAction()
    {
        
    }

    
    private void OnEnterSwordAttackAction()
    {
        
    }

    public bool GetIsInAttackRange()
    {
        return this.isInAttackRange;
    }

    public bool GetCanSwordAttack()
    {
        return this.canSwordAttack;
    }

    public void SetIsAttacking( bool value )
    {
        this.isAttacking = value;
    }

    public bool GetIsAttacking()
    {
        return this.isAttacking;
    }



}
