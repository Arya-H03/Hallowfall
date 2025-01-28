
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAttackState : EnemyBaseState
{
  

    [SerializeField] private bool  canAttack = true;  
    [SerializeField] private bool isAttacking = false;

    private EnemyBaseAttack attackRef;

    public bool IsAttaking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public EnemyBaseAttack AttackRef { get => attackRef; set => attackRef = value; }

    //private float moveSpeed = 2.5f;

    public EnemyAttackState() : base()
    {
        stateEnum = EnemyStateEnum.Attack;

    }

    private void Awake()
    {
        AttackRef = GetComponent<EnemyBaseAttack>();    
    }

    public override void OnEnterState()
    {
      
    }

    public override void OnExitState()
    {
        if (IsAttaking)
        {
            CancelAttack();
        }
    }

    public override void HandleState()
    {
        if (Vector2.Distance(enemyController.player.transform.position, this.transform.position) < AttackRef.AttackRange)
        {
            if(canAttack)
            {
                StartCoroutine(StartAttackCoroutine());
            }
            
            
        }
        else  
        {
            if (!isAttacking) 
            {
                
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }
               
        }
    }

    private IEnumerator StartAttackCoroutine()
    {
        if (enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x) 
        {
            
            enemyController.transform.localScale = new Vector3(-enemyController.transform.localScale.x, enemyController.transform.localScale.y, enemyController.transform.localScale.z);
           
            yield return new WaitForSeconds(Random.Range(0.75f,2.1f));


        }
        CanAttack = false;
        IsAttaking = true;
        enemyController.CanMove = false;

        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", true);
       
        yield return new WaitForSeconds(AttackRef.AttackCooldown);

        CanAttack = true;
        enemyController.CanMove = true;
    }
   
    public void EndAttack()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", false);
        IsAttaking = false;
        //enemController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    public void CancelAttack()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttacking", false);
        IsAttaking = false;
        //enemyController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);
        
    }
    public void CallAttack()
    {
       AttackRef.HandleAttack();
    }

    

}
