using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbilityHandler : MonoBehaviour
{
    [SerializeField] PassiveAbility healthIncrease;
    [SerializeField] PassiveAbility speedIncrease;

    PlayerController playerController;
    private void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();

        healthIncrease.passiveAbilityEvent += IncreaseHealth;
        speedIncrease.passiveAbilityEvent += IncreaseSpeed;
    }

    private void IncreaseHealth()
    {
        playerController.MaxHealth += (int)healthIncrease.modifier;
    }

    private void IncreaseSpeed()
    {
        //PlayerRunState playerRunState = playerController.StateMachine.PlayerRunState;
        ///playerRunState.RunSpeed *= 1 + speedIncrease.modifier;
    }

}
