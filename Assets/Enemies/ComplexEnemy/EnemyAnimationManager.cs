using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator animator;
    private EnemyController enemyController;


    private void Awake()
    {
        enemyController = GetComponent<EnemyController>(); 
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

        enemyController.EnemyMovement.OnEnemyEndTurning();
        
    }

    public void EndSwordAttaking()
    {
        enemyController.SwordAttackState.EndSwordAttack();
    }

    public void BoxCastSwordAttack()
    {
        enemyController.SwordAttackState.EnableBoxCastingForSwordAttack();
    }

    public void BeginBlockingSword()
    {
        enemyController.BlockState.BeginBlockingSword();
    }
}
