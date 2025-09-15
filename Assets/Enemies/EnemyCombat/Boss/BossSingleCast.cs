using UnityEngine;

[CreateAssetMenu(fileName = "BossSingleCast", menuName = "Scriptable Objects/Boss Abilites/Boss Single Cast")]
public class BossSingleCast : BaseEnemyAbilitySO
{
    [SerializeField] GameObject projectile;
    [SerializeField] int spawnCount;
    
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
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject proj = Instantiate(projectile, spawnerTransform.SpawnTransform.position, Quaternion.identity);
            proj.GetComponent<BaseProjectile>().SetProjectileToHomingMovement(enemy.PlayerGO, 0.5f, 0.75f);
        }
       
    }

    public override void EndAbility(EnemyController enemy)
    {
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, false);
    }
}
