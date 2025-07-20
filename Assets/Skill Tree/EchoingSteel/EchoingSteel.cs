using UnityEngine;

[CreateAssetMenu(fileName = "EchoingSteel", menuName = "SkillSO/EchoingSteel")]
public class EchoingSteel : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        SkillEvents.UnlockEchoingSteel();
    }
}
