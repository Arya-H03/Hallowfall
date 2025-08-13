

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyAttackState : EnemyState
{
    private EnemySignalHub signalHub;
    private float attackDelay;

    private List<BaseEnemyAbilitySO> abilityList;
    private List<BaseEnemyAbilitySO> availaleAbilityList;
    private BaseEnemyAbilitySO currentAbility;
    public EnemyAttackState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        signalHub = enemyController.SignalHub;


        abilityList = enemyController.GetListOfAllAbilities();
        availaleAbilityList = new List<BaseEnemyAbilitySO>(abilityList);
        currentAbility = MyUtils.GetRandomRef<BaseEnemyAbilitySO>(availaleAbilityList);

        signalHub.OnAbilityFinished += (value) => { EndAttack(); };
        signalHub.OnAbilityStart += SetupNextAbility;
    }

    public override void EnterState()
    {

        if (enemyController.IsAttackDelayOver && availaleAbilityList.Count > 0)
        {
            enemyController.CanMove = false;
            enemyController.CanAttack = false;
            enemyController.IsAttacking = true;

            signalHub.OnAbilityStart.Invoke(currentAbility);

            enemyController.CoroutineRunner.RunCoroutine(PutAbilityOnCooldownCoroutine(currentAbility));


        }
        else stateMachine.ChangeState(EnemyStateEnum.Idle);

    }

    public override void ExitState()
    {
        enemyController.CanMove = true;
        enemyController.CanAttack = true;
        enemyController.IsAttacking = false;

        currentAbility?.EndAbility(enemyController);
    }
    public void EndAttack()
    {
        signalHub.OnAbilityAnimFrame -= currentAbility.ActionOnAnimFrame;
        signalHub.OnAbilityFinished -= currentAbility.EndAbility;
        currentAbility = null;
        TrySetcurrentAbiliy();
        enemyController.CoroutineRunner.RunCoroutine(AttackDelayCoroutine());
        stateMachine.ChangeState(EnemyStateEnum.Idle);
    }

    private void SetupNextAbility(BaseEnemyAbilitySO ability)
    {
        signalHub.OnAbilityAnimFrame += ability.ActionOnAnimFrame;
        signalHub.OnAbilityFinished += ability.EndAbility;
        ability.ExecuteAbility(enemyController);

    }

    private IEnumerator PutAbilityOnCooldownCoroutine(BaseEnemyAbilitySO ability)
    {
        availaleAbilityList.Remove(ability);
        yield return new WaitForSeconds(ability.cooldown);
        availaleAbilityList.Add(ability);
        TrySetcurrentAbiliy();
    }
    private IEnumerator AttackDelayCoroutine()
    {
        attackDelay = Random.Range(enemyConfig.minAttackDelay, enemyConfig.maxAttackDelay + 0.1f);
        enemyController.IsAttackDelayOver = false;
        yield return new WaitForSeconds(attackDelay);
        enemyController.IsAttackDelayOver = true;
    }

    private void TrySetcurrentAbiliy()
    {
        if (availaleAbilityList.Count > 0)
        {
            BaseEnemyAbilitySO abilityWithBiggestRange =  null;
            foreach (var ability in availaleAbilityList)
            {
                if(abilityWithBiggestRange == null ) abilityWithBiggestRange = ability;
                else if (ability.range > abilityWithBiggestRange.range ) abilityWithBiggestRange = ability;
            }
            currentAbility = abilityWithBiggestRange;
        }
    }
    private bool IsEnemyInAttackRange()
    {
        return Vector2.Distance(enemyController.PlayerController.GetPlayerPos(), enemyController.GetEnemyPos()) <= currentAbility.range;
    }

    private bool IsEnemyAbleToAttack()
    {
        return (enemyController.CanAttack && !enemyController.IsAttacking && enemyController.IsAttackDelayOver);
    }



    public bool CanChangeToAttackState()
    {
        return IsEnemyInAttackRange() && IsEnemyAbleToAttack() && currentAbility != null;
    }

    public bool CanChasePlayerToAttack()
    {
        return IsEnemyAbleToAttack() && currentAbility != null;
    }

}
