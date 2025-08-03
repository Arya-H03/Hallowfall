using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MomentumShift", menuName = "SkillSO/MomentumShift")]
public class MomentumShift : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyParried += (EC, F) => { playerController.CoroutineRunner.StartCoroutine(MonentumShiftCoroutine(playerController)); };
    }
    private IEnumerator MonentumShiftCoroutine(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnChangeMoveSpeed?.Invoke(1.25f);
        yield return new WaitForSeconds(3f);
        playerController.PlayerSignalHub.OnChangeMoveSpeed?.Invoke(1f);

    }
}
