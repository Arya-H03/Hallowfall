using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerState
{
    #region Fields and Properties
     
     private PlayerParryShield parryShield;
     private GameObject impactEffectPrefab;
     private AudioClip  parrySFX;
     private float parryWindow;


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

    }

    public override void EnterState()
    {
        playerController.IsParrying = true;
        StartParry();
    }

    public override void ExitState()
    {
        playerController.IsParrying = false;
    }

  
    #endregion

    #region Parry Logic

    private void StartParry()
    {    
        signalHub.OnAnimTrigger?.Invoke("Parry");
        signalHub.OnAnimBool?.Invoke("isParrying", true); 
        
        playerController.CoroutineRunner.RunCoroutine(StopParryCoroutine());
    }

    private IEnumerator StopParryCoroutine()
    {
        yield return new WaitForSeconds(parryWindow);
        signalHub.OnAnimBool?.Invoke("isParrying", false);
        OnParryEnd();
    }

    private void OnParryEnd()
    {
        parryShield.BoxCollider.enabled = false;
        signalHub.OnAnimBool?.Invoke("isParrySuccessful", false);
        signalHub.OnStateTransitionBasedOnMovement?.Invoke(PlayerStateEnum.Parry);
    }

    private void ActivateParryShield()
    {
        parryShield.BoxCollider.enabled = true;
    }

    //public void CallOnParrySuccessfulEvent()
    //{
    //    OnParrySuccessful?.Invoke();
    //}

    private void OnSuccessfulParry(EnemyController enemyController, float parryDamage)
    {
        signalHub.OnAnimBool?.Invoke("isParrySuccessful", true);
        signalHub.OnSpawnVFX?.Invoke(impactEffectPrefab, playerController.GetPlayerPos(), Quaternion.identity, 0.3f);
        signalHub.OnPlaySFX?.Invoke(parrySFX, 0.5f);
        if (CanCounterParry) enemyController.SignalHub.OnEnemyHit?.Invoke(parryDamage, HitSfxType.sword, playerController.GetPlayerPos(), 2);
    }

    #endregion
}
