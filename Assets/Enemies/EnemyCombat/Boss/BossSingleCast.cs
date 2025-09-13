using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRangedAttack", menuName = "Scriptable Objects/Boss Abilites/Boss Single Cast")]
public class BossSingleCast : BaseEnemyAbilitySO
{
    [SerializeField] GameObject projectileTop;
    [SerializeField] GameObject projectileDown;
    [SerializeField] GameObject projectileLeft;
    [SerializeField] GameObject projectileRight;
    [SerializeField] GameObject projectileTopRight;
    [SerializeField] GameObject projectileTopLeft;
    [SerializeField] GameObject projectileDownRight;
    [SerializeField] GameObject projectileDownLeft;



    private CSpawnerTransform spawnerTransform;
    public override void ExecuteAbility(EnemyController enemy)
    {
        if (spawnerTransform == null)
        {
            if (!enemy.gameObject.TryGetComponent<CSpawnerTransform>(out CSpawnerTransform transform)) return;
            spawnerTransform = transform;
        }

        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, true);
        Vector2 dir = enemy.PlayerController.GetPlayerPos() - enemy.GetEnemyPos();
        int xDir = dir.x >= 0 ? -1 : 1;
        enemy.SignalHub.OnEnemyTurn?.Invoke(xDir);
    }
    public override void ActionOnAnimFrame(EnemyController enemy)
    {
        enemy.SignalHub.OnPlayRandomSFX?.Invoke(abilitySFX, 0.075f);
        GameObject proj = Instantiate(projectileTop, spawnerTransform.SpawnTransform.position, Quaternion.identity);
        proj.GetComponent<BaseProjectile>().SetProjectileToHomingMovement(enemy.PlayerGO,0.5f,1);
        GameObject proj1 = Instantiate(projectileTop, spawnerTransform.SpawnTransform.position, Quaternion.identity);
        proj1.GetComponent<BaseProjectile>().SetProjectileToHomingMovement(enemy.PlayerGO, 0.5f, 1);
        GameObject proj2 = Instantiate(projectileTop, spawnerTransform.SpawnTransform.position, Quaternion.identity);
        proj2.GetComponent<BaseProjectile>().SetProjectileToHomingMovement(enemy.PlayerGO, 0.5f, 1);
    }

    public override void EndAbility(EnemyController enemy)
    {
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, false);
    }
}
