using UnityEngine;

[CreateAssetMenu(fileName = "BurriedShadowUpgrade", menuName = "Scriptable Objects/ShadowStrikeUpgrades/BurriedShadow")]
public class BurriedShadowUpgrade : PlayerAbilityUpgradeSO
{
    public override void ApplyUpgradeLogicTo(IAbility ability)
    {
        if (ability is PlayerShadowStrikeAbility shadowStrikeAbility)
        {
            shadowStrikeAbility.ShadowCloneDamage += 10;
        }
    }
}