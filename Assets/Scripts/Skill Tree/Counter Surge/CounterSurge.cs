using UnityEngine;

[CreateAssetMenu(fileName = "CounterSurge", menuName = "SkillSO/Counter Surge")]
public class CounterSurge : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        SkillEvents.UnlockCounterSurge();
    }
}
