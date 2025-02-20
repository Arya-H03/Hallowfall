
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyAttackState : EnemyBaseState
{
    public enum AttackTypeEnum
    {
        regular,
        special,
    }

    [SerializeField] private bool  canAttack = true;  
    [SerializeField] private bool  canSpecialAttack = true;  
    [SerializeField] private bool isAttacking = false;

    private float attackDelay = 0;
    [SerializeField] float minAttackDelay = 2f;
    [SerializeField] float maxAttackDelay = 4f;
    [SerializeField] bool isAttackDelayOver = true;

    [SerializeField]  EnemyBaseAttack attackRef;
    [SerializeField]  EnemySpecialAbility specialAbilityRef;

    public List<EnemyBaseAttack> enemyAttackList;
    public List<EnemyBaseAttack> enemyAvailableAttackList;

    public bool IsAttaking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public EnemyBaseAttack AttackRef { get => attackRef; set => attackRef = value; }
    public EnemySpecialAbility SpecialAbilityRef { get => specialAbilityRef; set => specialAbilityRef = value; }
    public bool CanSpecialAttack { get => canSpecialAttack; set => canSpecialAttack = value; }
    public bool IsAttackDelayOver { get => isAttackDelayOver; set => isAttackDelayOver = value; }

    //private float moveSpeed = 2.5f;

    public EnemyAttackState() : base()
    {
        stateEnum = EnemyStateEnum.Attack;

    }

    private void Awake()
    {
        enemyAttackList = GetComponents<EnemyBaseAttack>().ToList();


        if (!specialAbilityRef)
        {          
            canSpecialAttack = false;
        }

    }
    private void Start()
    {
        enemyController.NavAgent.stoppingDistance = attackRef.AttackRange;
        attackDelay = Random.Range(minAttackDelay, maxAttackDelay + 0.1f);
    }
    public override void OnEnterState()
    {
      
    }

    public override void OnExitState()
    {
        if (IsAttaking)
        {
            CancelAttack("isAttacking");
            CancelAttack("isSpecialAttacking");
        }
    }

    public override void HandleState()
    {
        if (SpecialAbilityRef && Vector2.Distance(enemyController.PlayerPos, this.transform.position) < specialAbilityRef.AttackRange && CanSpecialAttack && !isAttacking && IsAttackDelayOver)
        {
            StartCoroutine(AttackDelayCoroutine());
            StartCoroutine(StartAttackCoroutine(AttackTypeEnum.special));

        }

        else if (Vector2.Distance(enemyController.PlayerPos, this.transform.position) < AttackRef.AttackRange && canAttack && !isAttacking && IsAttackDelayOver)
        {
            StartCoroutine(AttackDelayCoroutine());
            StartCoroutine(StartAttackCoroutine(AttackTypeEnum.regular));


        }
        else if (!isAttacking) enemyController.ChangeState(EnemyStateEnum.Chase);

        
    }

    private IEnumerator StartAttackCoroutine(AttackTypeEnum attacktype)
    {
        //StartCoroutine(AttackDelayCoroutine());  
        if (enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x) 
        {
            
            enemyController.transform.localScale = new Vector3(-enemyController.transform.localScale.x, enemyController.transform.localScale.y, enemyController.transform.localScale.z);
           
            yield return new WaitForSeconds(Random.Range(0.75f,2.1f));


        }

        IsAttaking = true;
        enemyController.CanMove = false;
      
         
        switch (attacktype)
        {
            case AttackTypeEnum.regular:
                CanAttack = false;
                enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", true);
                enemyController.CanMove = true;

                yield return new WaitForSeconds(attackRef.AttackCooldown);
                CanAttack = true;

                break;
            case AttackTypeEnum.special:
                CanSpecialAttack = false;
                enemyController.EnemyAnimationManager.SetBoolForAnimation("isSpecialAttacking", true);
                enemyController.CanMove = true;
                yield return new WaitForSeconds(specialAbilityRef.AttackCooldown);
                CanSpecialAttack = true;

                break;
        }  

        
        
       
    }
   
    public void EndAttack(AttackTypeEnum attacktype)
    {
        switch (attacktype)
        {
            case AttackTypeEnum.regular:
                enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", false);
                break;
            case AttackTypeEnum.special:
                enemyController.EnemyAnimationManager.SetBoolForAnimation("isSpecialAttacking", false);
                break;
        }
      
        IsAttaking = false;
        //enemController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    public void CancelAttack(string animBoolName)
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation(animBoolName, false);
        IsAttaking = false;
        //enemyController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);
        
    }
    public void CallAttack(AttackTypeEnum attacktype)
    {
        switch (attacktype)
        {
            case AttackTypeEnum.regular:
                AttackRef?.StartAttack();
                break;
            case AttackTypeEnum.special:
                specialAbilityRef?.StartAttack();
                break;
        }
       
    }

    private IEnumerator AttackDelayCoroutine()
    {
        IsAttackDelayOver = false;
        yield return new WaitForSeconds(attackDelay);
        IsAttackDelayOver = true;
    }

    public bool IsEnemyInAttackRange()
    {
        return Vector2.Distance(enemyController.PlayerPos, enemyController.transform.position) < AttackRef.AttackRange;

    }

    public bool IsEnemyAbleToAttaack()
    {
        return (canAttack || canSpecialAttack) && isAttackDelayOver;
    }
}
