using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour, IInitializeable<EnemyController>
{
    private Animator animator;
    private RuntimeAnimatorController runtimeAnimatorController;
    private EnemyController enemyController;
    private EnemyStateMachine stateMachine;

    private float hitAnimDuration;
    public Animator Animator { get => animator; set => animator = value; }
    public float HitAnimDuration { get => hitAnimDuration; set => hitAnimDuration = value; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        runtimeAnimatorController = animator.runtimeAnimatorController;
    }

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.stateMachine = enemyController.EnemyStateMachine;
    }
    private void Start()
    {
        foreach (AnimationClip clip in runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Hit") 
            {
                hitAnimDuration = clip.length;
            }
        }
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
 

