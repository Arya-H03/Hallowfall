using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedIncrease", menuName = "SpeedIncrease")]
public class SpeedIncrease : PassiveAbility
{
    public override void ApplyAbility()
    {
        PlayerRunState runState = GameManager.Instance.Player.GetComponentInChildren<PlayerRunState>();
        runState.RunSpeed *= (1 + modifier);
       
    }
}
