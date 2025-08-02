using System.Collections;
using System.Collections.Generic;
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


    public void FirstAttackSwing()
    {
      signalHub.OnFirstSwordSwing?.Invoke();
    }

    public void SecondAttackSwing()
    {
        signalHub.OnSecondSwordSwing?.Invoke();
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

}
