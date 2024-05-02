using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator animator;
    private EnemyStatesManager statesManager;


    private void Awake()
    {
        statesManager = GetComponent<EnemyStatesManager>(); 
        animator = GetComponent<Animator>();
    }
    public void SetBoolForAnimation(string name, bool value)
    {
        animator.SetBool(name, value);
        
    }

    public void SetTriggerForAnimation(string name)
    {
        animator.SetTrigger(name);

    }

    public void EndTurningAnimation()
    {

        statesManager.enemyMovement.OnEnemyEndTurning(false);
        
    }

    public void EndSwordAttaking()
    {
        statesManager.GetState(EnemyStateEnum.Attack).GetComponent<AttackState>().EndSwordAttack();
    }

    public void BoxCastSwordAttack()
    {
        statesManager.GetState(EnemyStateEnum.Attack).GetComponent<AttackState>().EnableBoxCastingForSwordAttack();
    }
}
