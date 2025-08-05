using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerState
{
    #region Fields and Properties
     
     private PlayerParryShield parryShield;
     private GameObject impactEffectPrefab;
     private AudioClip  [] parrySFX;
     private float parryWindow;

    private bool isParrySuccessful = false;
    private bool canCounterParry = true;
    private bool canParryProjectiles = false;

    public bool CanCounterParry { get => canCounterParry; set => canCounterParry = value; }
    public bool CanParryProjectiles { get => canParryProjectiles; set => canParryProjectiles = value; }

    #endregion


    #region State Methods

    public PlayerParryState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Parry;
        impactEffectPrefab = playerConfig.impactEffectPrefab;
        parrySFX = playerConfig.parrySFX;
        parryWindow = playerConfig.parryWindow;
        parryShield = playerController.ParryShield;

        signalHub.OnActivatingParryShield += ActivateParryShield;
        signalHub.OnEnemyParried += OnSuccessfulParry;
        signalHub.OnEnemyParried += OnSuccessfulParry;
        signalHub.OnParryEnd += OnParryHoldAnimEnd;

    }

    public override void EnterState()
    {
        playerController.IsParrying = true;
        StartParry();
    }

    public override void ExitState()
    {
        
        playerController.IsParrying = false;
        isParrySuccessful = false;
    }

  
    #endregion

    #region Parry Logic

    private void StartParry()
    {
        signalHub.OnTurningToMousePos?.Invoke();
        signalHub.OnAnimBool?.Invoke("isParrying",true);
        
        playerController.CoroutineRunner.RunCoroutine(StopParryCoroutine());
    }

    private IEnumerator StopParryCoroutine()
    {
        yield return new WaitForSeconds(parryWindow);
        OnParryHoldAnimEnd();
    }

    private void OnParryHoldAnimEnd()
    {
        parryShield.BoxCollider.enabled = false;
        if (!isParrySuccessful)
        {
            signalHub.OnAnimBool?.Invoke("isParrying", false);
            signalHub.OnAnimBool?.Invoke("isParrySuccessful", false);
            signalHub.OnStateTransitionBasedOnMovement?.Invoke(PlayerStateEnum.Parry);
        }
        else isParrySuccessful = false;
    }

    private void ActivateParryShield()
    {
        parryShield.BoxCollider.enabled = true;
    }

    private void OnSuccessfulParry(EnemyController enemyController, float parryDamage)
    {
        
        parryShield.BoxCollider.enabled = false;
        signalHub.OnPlayRandomSFX?.Invoke(parrySFX, 1f);
        if (CanCounterParry)
        {
            isParrySuccessful = true;
            signalHub.OnAnimBool?.Invoke("isParrySuccessful", true);
         
        }
            
        //signalHub.OnSpawnVFX?.Invoke(impactEffectPrefab, playerController.GetPlayerPos(), Quaternion.identity, 0.3f);
    }

 

    #endregion
}
