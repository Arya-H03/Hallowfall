using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator animator;
    private EnemyController statesManager;


    private void Awake()
    {
        statesManager = GetComponent<EnemyController>(); 
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

        statesManager.enemyMovement.OnEnemyEndTurning();
        
    }

    public void EndSwordAttaking()
    {
        statesManager.GetState(EnemyStateEnum.Combat).GetComponent<CombatState>().EndSwordAttack();
    }

    public void BoxCastSwordAttack()
    {
        statesManager.GetState(EnemyStateEnum.Combat).GetComponent<CombatState>().EnableBoxCastingForSwordAttack();
    }

    public void BeginBlockingSword()
    {
        statesManager.GetState(EnemyStateEnum.Block).GetComponent<BlockState>().BeginBlockingSword();
    }
}
