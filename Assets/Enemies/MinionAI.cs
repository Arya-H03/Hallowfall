using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAI : EnemyAI

{
    private MinionAttack minionAttack;

    protected override void Awake()
    {
        base.Awake();
        minionAttack = GetComponent<MinionAttack>();
    }

    #region AttackState
    protected override void HandleAttackState()
    {
        if (!isDead)
        {
            if (Vector2.Distance(transform.position, player.transform.position) < attackRange)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", true);

            }

            else
            {
                EndAttackAnim();
                currentState = EnemyState.Chase;
                animator.SetBool("isRunning", true);
            }
        }

    }

    public void BoxCastForAttack()
    {
        minionAttack.BoxCast();

    }

    public override void EndAttackAnim()
    {
        animator.SetBool("isAttacking", false);
        
    }
    #endregion
}
