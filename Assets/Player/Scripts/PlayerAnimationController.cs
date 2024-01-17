using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] PlayerFootSteps footSteps;
    [SerializeField] PlayerController playerController;
    [SerializeField] RuntimeAnimatorController OneHandAc;
    [SerializeField] RuntimeAnimatorController TwoHandAc;


    [SerializeField] GameObject LeftHand;

    private string [] animConditions = {"isRunning","isJumping","isHit"};   

    private void OnEnable()
    {
        //PlayerController.ShootGunEvent += ChangeAnimatorAC;
    }

    private void OnDisable()
    {
        //PlayerController.ShootGunEvent -= ChangeAnimatorAC;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //public void ChangeAnimatorAC()
    //{
    //    animator.runtimeAnimatorController = OneHandAc;
    //    LeftHand.SetActive(true);
    //}

    public void SetBoolForAnimations(string name,bool value)
    {
        animator.SetBool(name,value);
    }

    public void SetTriggerForAnimations(string name)
    {
        animator.SetTrigger(name);
    }

    public void ManageParryAttack()
    {
        SetBoolForAnimations("isParrySuccessful", false);
    }

    public void CreateFootStepDust()
    {
        //footSteps.PlayFootstepPSEffect();
    }

    //public void EndisHit()
    //{
    //    SetBoolForAnimations("isHit", false);
    //}


    public void StopAttack()
    {
        playerController.playerAttacks.EndAttack();
    }

   
    public void BoxCastForAttack1()
    {
        playerController.playerAttacks.Attack1BoxCast();
    }

    public void BoxCastForAttack2()
    {
        playerController.playerAttacks.Attack2BoxCast();
    }

    public void BoxCastForAttack3()
    {
        playerController.playerAttacks.Attack3BoxCast();
    }
    public void OnParryEnd()
    {
        playerController.playerParry.OnParryEnd();
    }
    public void ActivateParryShield()
    {
        playerController.playerParry.ActivateParryShield();
    }

    public void EndAnimations(string name)
    {
        foreach(string condition in animConditions)
        {
            if(condition != name)
            {
                animator.SetBool(condition, false);
            }      
        }
    }
}
