using UnityEngine;

[CreateAssetMenu(fileName = "CounterSurge", menuName = "SkillSO/Counter Surge")]
public class CounterSurge : SkillSO
{
    public float restoreAmount = 10;
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyParried += (EC,F) => { playerController.PlayerSignalHub.OnRestoreHealth?.Invoke(restoreAmount); };
    }
}
