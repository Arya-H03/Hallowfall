using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerDeathState : PlayerBaseState
{

    public static event MyEventHandler PlayerDeathEvent;
    public static event MyEventHandler PlayerRespawnEvent;
    public PlayerDeathState()
    {
        this.stateEnum = PlayerStateEnum.Death;
    }

    private void OnEnable()
    {
        PlayerDeathEvent += StartPlayerDeathCoroutine;
        PlayerRespawnEvent += PlayerRespawn;
    }

    private void OnDisable()
    {
        PlayerDeathEvent -= StartPlayerDeathCoroutine;
        PlayerRespawnEvent -= PlayerRespawn;
    }
    public override void OnEnterState()
    {
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;
        OnPlayerDeath();
    }

    public override void OnExitState()
    {
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;
    }

    public override void HandleState()
    {


    }

    private void StartPlayerDeathCoroutine()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }

    private IEnumerator PlayerDeathCoroutine()
    {
        InputManager.Instance.OnDisable();
        playerController.deathEffectParticle.Play();
        playerController.IsDead = true;    
        playerController.AnimationController.SetBoolForAnimations("isDead", true);
        playerController.AnimationController.SetTriggerForAnimations("Death");
        GameManager.Instance.DistortCamera();

        yield return new WaitForSeconds(1f);

        GameManager.Instance.OpenDeathMenu();

    }

    public void OnPlayerDeath()
    {
        if (!playerController.IsDead)
        {
            PlayerDeathEvent?.Invoke();
        }

    }

    private void PlayerRespawn()
    {
        GameManager.Instance.EndPlayerDistortion();
        GameManager.Instance.LastStatue.SetPlayerPositionOnRespawn(this.transform.parent.parent.gameObject );
        playerController.AnimationController.SetBoolForAnimations("isDead", false);
        InputManager.Instance.OnEnable();
        playerController.IsDead = false;
        playerController.ChangeState(PlayerStateEnum.Idle); 

    }

    public void OnPlayerRespawn()
    {
        if (playerController.IsDead)
        {
            PlayerRespawnEvent?.Invoke();
        }
       
    }
}
