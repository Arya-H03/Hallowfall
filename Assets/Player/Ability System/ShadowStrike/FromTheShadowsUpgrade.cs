
using UnityEngine;

[CreateAssetMenu(fileName = "FromTheShadowsUpgrade", menuName = "Scriptable Objects/ShadowStrikeUpgrades/FromTheShadows")]
public class FromTheShadowsUpgradeData : PlayerAbilityUpgradeSO
{
    public override void ApplyUpgradeLogicTo(IAbility ability)
    {
        if (ability is PlayerShadowStrikeAbility shadowStrikeAbility)
        {
            shadowStrikeAbility.SpawnCount++;
        }
    }
}