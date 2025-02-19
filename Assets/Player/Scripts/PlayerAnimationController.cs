using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;

    [SerializeField] RuntimeAnimatorController withSwordAC;
    [SerializeField] RuntimeAnimatorController withoutSwordAC;


    [SerializeField] GameObject LeftHand;

    private string[] animConditions = { "isRunning", "isJumping", "isHit" };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    public void ChangeAnimatorAC(bool hasSword)
    {
        if (hasSword)
        {
            animator.runtimeAnimatorController = withSwordAC;
        }

        if (!hasSword)
        {
            animator.runtimeAnimatorController = withoutSwordAC;
        }

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

    public void SetPlayerFallStatus()
    {
        playerController.PlayerJumpState.SetPlayerFallStatus();
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
