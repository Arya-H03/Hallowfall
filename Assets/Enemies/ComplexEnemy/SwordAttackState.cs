using Google.Protobuf.WellKnownTypes;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Barracuda;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class SwordAttackState : EnemyBaseState
{
  
    private float swordAttackCooldown = 4f;

    [SerializeField] private float attackRange = 2f;

    [SerializeField] private bool  canSwordAttack = true;  
    [SerializeField] private bool isSwordAttaking = false;


    private SwordAttack swordAttack;

    public float AttackRange { get => attackRange; set => attackRange = value; }
    public bool IsSwordAttaking { get => isSwordAttaking; set => isSwordAttaking = value; }
    public bool CanSwordAttack { get => canSwordAttack; set => canSwordAttack = value; }

    //private float moveSpeed = 2.5f;

    public SwordAttackState() : base()
    {
        stateEnum = EnemyStateEnum.SwordAttack;

    }

    private void Awake()
    {
        swordAttack = GetComponentInChildren<SwordAttack>();
    }

    public override void OnEnterState()
    {
        StartCoroutine(SwordAttackCoroutine());
    }

    public override void OnExitState()
    {
        if (IsSwordAttaking)
        {
            CancelSwordAttack();
        }
    }

    public override void HandleState()
    {

    }

    private IEnumerator SwordAttackCoroutine()
    {
        
        CanSwordAttack = false;
        IsSwordAttaking = true;
        enemyController.CanMove = false;

        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", true);
       
        yield return new WaitForSeconds(swordAttackCooldown);
        CanSwordAttack = true;
        enemyController.CanMove = true;
    }

   
    public void EndSwordAttack()
    {
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);
        IsSwordAttaking = false;
        //enemyController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    public void CancelSwordAttack()
    {
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);
        IsSwordAttaking = false;
        //enemyController.CanMove = true;
        enemyController.ChangeState(EnemyStateEnum.Idle);
        
    }
    public void EnableBoxCastingForSwordAttack()
    {
        
        swordAttack.SwordAttackBoxCast();
    }

    

}
