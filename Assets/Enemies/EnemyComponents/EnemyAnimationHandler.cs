using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour, IInitializeable<EnemyController>
{
    private Animator animator;
    //private RuntimeAnimatorController runtimeAnimatorController;
    private EnemyStateMachine stateMachine;
    private EnemySignalHub signalHub;
    public Animator Animator { get => animator; set => animator = value; }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
        //runtimeAnimatorController = animator.runtimeAnimatorController;
    }

    public void Init(EnemyController enemyController)
    {
        this.stateMachine = enemyController.EnemyStateMachine;
        signalHub = enemyController.SignalHub;

        signalHub.OnEnemyHit += CallHitAnim;
        signalHub.OnEnemyDeath += CallDeathAnim;
    }

    private void OnDisable()
    {
        signalHub.OnEnemyHit -= CallHitAnim;
        signalHub.OnEnemyDeath -= CallDeathAnim;
    }
 

    private void CallHitAnim(float f, HitSfxType h)
    {
        animator.SetTrigger("Hit");
    }

    private void CallDeathAnim()
    {
        animator.SetTrigger("Death");
    }


    public void SetBoolForAnimation(string name, bool value)
    {
        Animator.SetBool(name, value);
        
    }

    public void SetTriggerForAnimation(string name)
    {
        Animator.SetTrigger(name);

    }

    public void PerformAttack()
    {
        stateMachine.AttackState.NextAttack.CallAttackActionOnAnimFrame();
    }

    public void OnEndAttackAnim()
    {
        stateMachine.AttackState.EndAttack();
    }

   

}


