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
    public Animator Animator { get => animator; set => animator = value; }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Init(EnemyController enemyController)
    {
        signalHub = enemyController.SignalHub;
        this.enemyController = enemyController;

        signalHub.OnEnemyHit += CallHitAnim;
        signalHub.OnEnemyDeath += CallDeathAnim;
    }

    private void OnDisable()
    {
        if (signalHub == null) return;
        signalHub.OnEnemyHit -= CallHitAnim;
        signalHub.OnEnemyDeath -= CallDeathAnim;
    }
 

    private void CallHitAnim(float f, HitSfxType h)
    {
        animator.SetTrigger("Hit");
    }

    private void CallDeathAnim()
    {
        animator.ResetTrigger("Hit");
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

   public float GetAnimationLength(string  name)
    {
        return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == name).length;
    }

}


