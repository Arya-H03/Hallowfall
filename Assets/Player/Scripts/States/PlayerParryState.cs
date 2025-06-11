using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    [SerializeField] private float moveSpeedWhileParrying = 2;
    private bool canCounterParry = false;

    [SerializeField] GameObject parryShield;

    public bool CanCounterParry { get => canCounterParry;}

    public PlayerParryState()
    {
        this.stateEnum = PlayerStateEnum.Parry;
    }
    private void OnEnable()
    {
        SkillEvents.OnCounterSkillUnlocked += CounterSkillLogic;
        
    }
    private void OnDisable()
    {
        SkillEvents.OnCounterSkillUnlocked -= CounterSkillLogic;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileParrying;
        StartParry();
    }

    public override void OnExitState()
    {
        playerController.PlayerMovementManager.MoveSpeed = playerController.PlayerMovementManager.MoveSpeed;
       
    }

    public override void HandleState()
    {


    }

    private void StartParry()
    {

        playerController.IsParrying = true;
        playerController.CanPlayerJump = false;
        playerController.AnimationController.SetTriggerForAnimations("Parry");
        playerController.AnimationController.SetBoolForAnimations("isParrying",true);
        StartCoroutine(StopParryCoroutine());
       
        
    }

    private IEnumerator StopParryCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        playerController.AnimationController.SetBoolForAnimations("isParrying", false);
        OnParryEnd();
    }

    public void ActivateParryShield()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = true;
    }


    public void OnParryEnd()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = false;
        playerController.IsParrying = false;
        playerController.CanPlayerJump = true;
        playerController.AnimationController.SetBoolForAnimations("isParrySuccessful", false);
        if (playerController.PlayerMovementManager.currentInputDir.x != 0)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
        
    }

    private void CounterSkillLogic()
    {
        canCounterParry = true;
    }
}
