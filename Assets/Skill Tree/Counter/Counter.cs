using UnityEngine;

[CreateAssetMenu(fileName = "CounterSkill", menuName = "SkillSO/Counter")]
public class Counter : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.StateMachine.PlayerParryState.CanCounterParry = true;
    }
}
