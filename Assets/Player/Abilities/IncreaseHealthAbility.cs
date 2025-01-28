using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthIncrease", menuName = "HealthIncrease")]
public class IncreaseHealthAbility : PassiveAbility
{
    public override void ApplyAbility()
    {
        PlayerController playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        playerController.PlayerInfo.MaxHealth += (int)modifier;
        GameManager.Instance.healthText.text = playerController.PlayerInfo.CurrentHealth.ToString() + "/" + playerController.PlayerInfo.MaxHealth.ToString();
    }
}
