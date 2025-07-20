using UnityEngine;

[CreateAssetMenu(fileName = "PerfectTiming", menuName = "SkillSO/PerfectTiming")]
public class PerfectTiming : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        SkillEvents.UnlockPerfectTiming();
    }
}
