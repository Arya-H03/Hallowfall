using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    
    public float swordAttackTimer = 0f;
    private float swordAttackCooldown = 2f;

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
        if (swordAttackTimer >= swordAttackCooldown)
        {
            BeginSwordAttack();
        }

        if (Vector2.Distance(statesManager.player.transform.position, gameObject.transform.position) >= 2)
        {
            statesManager.ChangeState(EnemyStateEnum.Chase);
        }
     // swordAttack.DrawCast();
    }

    private void BeginSwordAttack()
    {
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", true);
    }

    public void EndSwordAttack()
    {
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        swordAttackTimer = 0f;
    }

    public void CancelSwordAttack()
    {
        statesManager.animationManager.SetBoolForAnimation("isAttackingSword", false);
        swordAttackTimer = 1f;
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
