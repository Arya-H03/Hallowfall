using UnityEngine;

[CreateAssetMenu(fileName = "BladeReflection", menuName = "SkillSO/BladeReflection")]
public class BladeReflection : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.StateMachine.PlayerParryState.CanParryProjectiles = true;
    }
}
