using UnityEngine;

[CreateAssetMenu(fileName = "MomentumShift", menuName = "SkillSO/MomentumShift")]
public class MomentumShift : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        SkillEvents.UnlockMomentumShift();
    }
}
