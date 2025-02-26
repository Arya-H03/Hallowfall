
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyAttackState : EnemyBaseState
{
 

    [SerializeField] private bool  canAttack = true;  
    [SerializeField] private bool isAttacking = false;

    private float attackDelay = 0;
    [SerializeField] float minAttackDelay = 2f;
    [SerializeField] float maxAttackDelay = 4f;
    [SerializeField] bool isAttackDelayOver = true;

    private List<EnemyBaseAttack> enemyAttackList;
    private List<EnemyBaseAttack> enemyAvailableAttackList;
    private EnemyBaseAttack nextAttack;

    public bool IsAttaking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool IsAttackDelayOver { get => isAttackDelayOver; set => isAttackDelayOver = value; }
    public EnemyBaseAttack NextAttack { get => nextAttack; set => nextAttack = value; }

    public EnemyAttackState() : base()
    {
        stateEnum = EnemyStateEnum.Attack;

    }

    private void Awake()
    {
        enemyAttackList = GetComponents<EnemyBaseAttack>().ToList();
        enemyAvailableAttackList = new List<EnemyBaseAttack>(enemyAttackList);

    }
    private void Start()
    {
        nextAttack = enemyAvailableAttackList[0];
        //.enemyController.NavAgent.stoppingDistance = nextAttack.AttackRange;
        
    }
    public override void OnEnterState()
    {
        enemyController.CanMove = false;      
        if (isAttackDelayOver)
        {
            canAttack = false;
            isAttacking = true;
            enemyController.EnemyMovement.FacePlayer();
            nextAttack.StartAttack();
            StartCoroutine(AttackDelayCoroutine());
        }
        else enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    public override void OnExitState()
    {
        enemyController.CanMove = true;
        canAttack = true;
        isAttacking = false;
    }

    public override void HandleState()
    {
       
    }
   
    public void EndAttack()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation(nextAttack.AnimCondition, false);
        nextAttack = null;
        enemyController.ChangeState(EnemyStateEnum.Idle);
    }

    public void CancelAttack(string animBoolName)
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation(animBoolName, false);
        IsAttaking = false;
        enemyController.ChangeState(EnemyStateEnum.Idle);
        
    }
    private IEnumerator AttackDelayCoroutine()
    {
        attackDelay = Random.Range(minAttackDelay, maxAttackDelay + 0.1f);
        IsAttackDelayOver = false;
        yield return new WaitForSeconds(attackDelay);
        IsAttackDelayOver = true;
    }

    public bool IsEnemyInAttackRange()
    {
        return Vector2.Distance(enemyController.PlayerPos, enemyController.transform.position) <= nextAttack.AttackRange +1;
        //return Mathf.Abs(enemyController.PlayerPos.x - enemyController.transform.position.x) < nextAttack.AttackRange;

    }

    public bool IsEnemyAbleToAttack()
    {
        return (canAttack && !isAttacking && isAttackDelayOver);
    }

    public void AddToAvailableAttacks(EnemyBaseAttack attack)
    {
        enemyAvailableAttackList.Add(attack);
    }

    public void RemoveFromAvailableAttacks(EnemyBaseAttack attack)
    {
        enemyAvailableAttackList.Remove(attack);
    }

    public void CheckForNextAttack()
    {
        if (!nextAttack && enemyAvailableAttackList.Count > 0)
        {
            nextAttack = enemyAvailableAttackList[0];
            //enemyController.NavAgent.stoppingDistance = nextAttack.AttackRange;
        }
    }
}
