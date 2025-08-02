using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public delegate void MyEventHandler();

  
    public PlayerDeathState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Death;
        signalHub.OnDeath += StartPlayerDeathCoroutine;

    }

    public override void EnterState()
    {
        if (!playerController.IsDead)
        {
            signalHub.OnDeath?.Invoke();
        }
    }

    private void StartPlayerDeathCoroutine()
    {
        playerController.CoroutineRunner.RunCoroutine(PlayerDeathCoroutine());
       
    }

    private IEnumerator PlayerDeathCoroutine()
    {
        playerController.PlayerInputHandler.OnDisable();
        playerController.IsDead = true;
        signalHub.OnAnimBool?.Invoke("isDead", true);
        signalHub.OnAnimTrigger?.Invoke("Death");
      
        GameManager.Instance.DistortCamera();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.OpenDeathMenu();
    }

    //private void PlayerRespawn()
    //{
    //    GameManager.Instance.EndPlayerDistortion();
    //    playerController.PlayerAnimationHandler.SetBoolForAnimations("isDead", false);
    //    playerController.PlayerInputHandler.OnEnable();
    //    playerController.IsDead = false;
    //    playerController.PlayerHitHandler.RestoreFullHealth(playerController.MaxHealth);
    //    playerController.ResetPlayerVariables();
    //    signalHub.OnChangeState?.Invoke(PlayerStateEnum.Idle);

    //}

   

    //public void OnPlayerRespawn()
    //{
    //    if (playerController.IsDead)
    //    {
    //        PlayerRespawnEvent?.Invoke();
    //    }
       
    //}
}
