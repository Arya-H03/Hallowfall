using UnityEngine;

[CreateAssetMenu(fileName = "TryStagger", menuName = "SkillSO/TryStagger")]
public class EchoingSteel : SkillSO
{
    public override void ApplySkill(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyParried += (EC, F) => { TryStagger(EC); };
    }

    private void TryStagger(EnemyController enemyController)
    {
        enemyController.EnemyHitHandler.TryStagger(100);

    }
}
