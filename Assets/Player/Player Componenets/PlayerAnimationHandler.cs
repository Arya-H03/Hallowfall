using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.IK;

public class PlayerAnimationHandler : MonoBehaviour,IInitializeable<PlayerController>
{
    private Animator animator;
    private PlayerController playerController;
    private PlayerSignalHub signalHub;
    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        signalHub = playerController.PlayerSignalHub;
        animator = GetComponent<Animator>();

        signalHub.RequestAnimLength += GetAnimationLength;
        signalHub.OnAnimBool += SetBoolForAnimations;
        signalHub.OnAnimTrigger += SetTriggerForAnimations;
       
    }
    private void OnDisable()
    {
        signalHub.OnAnimBool -= SetBoolForAnimations;
        signalHub.OnAnimTrigger -= SetTriggerForAnimations;
    }

    public void SetBoolForAnimations(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetTriggerForAnimations(string name)
    {
        animator.SetTrigger(name);
    }

    public void EndSwordAttack()
    {
        signalHub.OnSwordSwingEnd?.Invoke();
    }


    public void OnHitFrameOfAttackAnim()
    {
      signalHub.OnSwordAttackHitFrame?.Invoke();
    }
    public void OnHitFrameOfParryAttackAnim()
    {
        signalHub.OnParryAttackHit?.Invoke();
    }

    public void OnEndFrameOfParryAttackAnim()
    {
        signalHub.OnParryEnd?.Invoke();
    }
    public void OnSFXFrameOfAttackAnim()
    {
        signalHub.OnSwordAttackSFXFrame?.Invoke();
    }
    //Gets call when entering the parry hold anim
    public void ActivateParryShield()
    {
        signalHub.OnActivatingParryShield?.Invoke();
    }

    //Gets Called during Run anim
    public void OnPlayerStep()
    {
        signalHub.OnPlayerStep?.Invoke();
    }

    private float GetAnimationLength(string name)
    {
        return animator.runtimeAnimatorController.animationClips.First(clip => clip.name == name).length;
    }
}
