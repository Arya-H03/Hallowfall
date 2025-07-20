using UnityEngine;

[CreateAssetMenu(fileName = "DoubleDashSkill", menuName = "SkillSO/DoubleDash")]
public class DoubleDashSkillSO : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        SkillEvents.UnlockDoubleDash();
    }
}
