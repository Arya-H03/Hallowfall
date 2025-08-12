using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour, IInitializeable<EnemyController>
{
    private Animator animator;
    EnemyController enemyController;
    private EnemySignalHub signalHub;
    private Animator Animator { get => animator; set => animator = value; }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Init(EnemyController enemyController)
    {
        signalHub = enemyController.SignalHub;
        this.enemyController = enemyController;

        signalHub.OnAnimTrigger += SetTriggerForAnimation;
        signalHub.OnResetAnimTrigger += ResetAnimTrigger;
        signalHub.OnAnimBool += SetBoolForAnimation;

        signalHub.RequestAnimLength += GetAnimationLength;
    }

    private void OnDisable()
    {
        if (signalHub == null) return;
        signalHub.OnAnimTrigger -= SetTriggerForAnimation;
        signalHub.OnAnimBool -= SetBoolForAnimation;
        signalHub.OnResetAnimTrigger -= ResetAnimTrigger;
        signalHub.RequestAnimLength -= GetAnimationLength;

    }

    private void ResetAnimTrigger(string name)
    {
        animator.ResetTrigger(name);
    }


    private void SetBoolForAnimation(string name, bool value)
    {
        Animator.SetBool(name, value);
        
    }

    private void SetTriggerForAnimation(string name)
    {
        Animator.SetTrigger(name);

    }

    //Gets called on the impact frame of the attack anim
    public void ActionOnAttackAnimFrame()
    {
        signalHub.OnAbilityAnimFrame?.Invoke(enemyController);
    }

    //Gets Called on the last frame of attack anim
    public void OnAttackAnimEnd()
    {
        signalHub.OnAbilityFinished?.Invoke(enemyController);
    }

    private float GetAnimationLength(string  name)
    {
        return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == name).length;
    }

}


