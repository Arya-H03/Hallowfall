using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    private string[] animConditions = { "isRunning", "isJumping", "isHit" };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponentInParent<PlayerController>();
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
        playerController.PlayerSwordAttackState.EndAttack();
    }


    public void FirstAttackSwing()
    {
       
        playerController.PlayerSwordAttackState.OnFirstSwordSwingEvent?.Invoke();
    }

    public void SecondAttackSwing()
    {
        
        playerController.PlayerSwordAttackState.OnSecondSwordSwingEvent?.Invoke();
    }

    public void ThirdAttackSwing()
    {
        playerController.PlayerSwordAttackState.OnThirdSwordSwingEvent?.Invoke();
    }

    public void OnParryEnd()
    {
        playerController.PlayerParryState.OnParryEnd();
    }
    public void ActivateParryShield()
    {
        playerController.PlayerParryState.ActivateParryShield();
    }

    public void EndAnimations(string name)
    {
        foreach (string condition in animConditions)
        {
            if (condition != name)
            {
                animator.SetBool(condition, false);
            }
        }
    }

  
    public void OnRollEnd()
    {
        playerController.PlayerRollState.EndRoll();
    }

    public void PlayStepSound()
    {
        playerController.PlayerRunState.PlayStepSound();
    }
}
