
using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VolumeComponent;
using Random = UnityEngine.Random;

public class EnemyAttackState : EnemyState
{
    private EnemyMovementHandler movementHandler;
    private EnemyAnimationHandler animationManager;
    private EnemySignalHub signalHub;

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
    public EnemyAttackState(EnemyController enemyController,EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.movementHandler = enemyController.EnemyMovementHandler;
        this.animationManager = enemyController.EnemyAnimationHandler;
        signalHub = enemyController.SignalHub;
        enemyAttackList = enemyController.Attacks; 

        attackDelay = 0;

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
            //movementHandler.CheckForFacingDirection(enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos());
            nextAttack.StartAttack();

            enemyController.CoroutineRunner.RunCoroutine(AttackDelayCoroutine());

        }
        else stateMachine.ChangeState(EnemyStateEnum.Idle);

    }

    public override void ExitState()
    {
        enemyController.CanMove = true;
        canAttack = true;
        isAttacking = false;
        IsAttackDelayOver = true;
        attackDelay = 0;
        if (nextAttack != null)
        {
            nextAttack.DeactivateZoneAttack();
        }

    }
    public void EndAttack()
    {
        animationManager.SetBoolForAnimation(nextAttack.AnimCondition, false);
        nextAttack.OnAttackEnd();
        nextAttack = null;
        stateMachine.ChangeState(EnemyStateEnum.Idle);
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
