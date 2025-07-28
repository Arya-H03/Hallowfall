
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

public class EnemyAttackState : EnemyState
{
    private bool canAttack = true;
    private bool isAttacking = false;
    bool isAttackDelayOver = true;

    private float attackDelay;

    private List<EnemyBaseAttack> enemyAttackList;
    private List<EnemyBaseAttack> enemyAvailableAttackList;
    private EnemyBaseAttack nextAttack;

    public bool IsAttaking { get => isAttacking; set => isAttacking = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool IsAttackDelayOver { get => isAttackDelayOver; set => isAttackDelayOver = value; }
    public EnemyBaseAttack NextAttack { get => nextAttack; set => nextAttack = value; }
    public EnemyAttackState(EnemyController enemyController, EnemyStateEnum stateEnum, EnemyConfigSO enemyConfig) : base(enemyController, stateEnum, enemyConfig)
    {
        attackDelay = 0;
        enemyAttackList = enemyController.Attacks;
        enemyAvailableAttackList = new List<EnemyBaseAttack>(enemyAttackList);
        nextAttack = enemyAvailableAttackList[0];
    }

    public override void EnterState()
    {
        enemyController.CanMove = false;
        if (isAttackDelayOver)
        {
            canAttack = false;
            isAttacking = true;
            enemyController.EnemyMovementHandler.CheckForFacingDirection(enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos());
            nextAttack.StartAttack();

            enemyController.CoroutineRunner.RunCoroutine(AttackDelayCoroutine());

        }
        else enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    public override void ExitState()
    {
        enemyController.CanMove = true;
        canAttack = true;
        isAttacking = false;
        if (nextAttack != null)
        {
            nextAttack.DeactivateZoneAttack();
        }

    }
    public void EndAttack()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation(nextAttack.AnimCondition, false);
        nextAttack.OnAttackEnd();
        nextAttack = null;
        enemyController.ChangeState(EnemyStateEnum.Idle);
    }

    private IEnumerator AttackDelayCoroutine()
    {
        attackDelay = Random.Range(enemyConfig.minAttackDelay, enemyConfig.maxAttackDelay + 0.1f);
        IsAttackDelayOver = false;
        yield return new WaitForSeconds(attackDelay);
        IsAttackDelayOver = true;
    }

    public bool IsEnemyInAttackRange()
    {
        return Vector2.Distance(enemyController.PlayerController.GetPlayerPos(), enemyController.GetEnemyPos()) <= nextAttack.AttackRange;
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
        }
    }
}
