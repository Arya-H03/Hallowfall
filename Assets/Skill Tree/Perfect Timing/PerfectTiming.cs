using UnityEngine;
using static PlayerDashState;

[CreateAssetMenu(fileName = "PerfectTiming", menuName = "SkillSO/PerfectTiming")]
public class PerfectTiming : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyParried += (EC, F) => { PerfectTimingSkillLogic(playerController); }; 
    }

    private void PerfectTimingSkillLogic(PlayerController playerController)
    {
        foreach (DashChargeSlot charge in playerController.StateMachine.PlayerDashState.UnAvailableDashCharges)
        {
            charge.DelayTimer += 1;
        }

    }


}
