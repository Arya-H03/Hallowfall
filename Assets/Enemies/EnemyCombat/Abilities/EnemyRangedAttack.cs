using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRangedAttack", menuName = "Scriptable Objects/Enemy Abilites/Enemy Ranged Attack")]
public class EnemyRangedAttack : BaseEnemyAbilitySO
{
    [SerializeField] GameObject projectile;
    private CSpawnerTransform spawnerTransform;
    public override void ExecuteAbility(EnemyController enemy)
    {
        if (spawnerTransform == null )
        {
            if (!enemy.gameObject.TryGetComponent<CSpawnerTransform>(out CSpawnerTransform transform)) return;
            spawnerTransform = transform;
        }
        
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition,true);
        Vector2 dir = enemy.PlayerController.GetPlayerPos() - enemy.GetEnemyPos();
        int xDir = dir.x >= 0 ? -1 : 1;
        enemy.SignalHub.OnEnemyTurn?.Invoke(xDir);
    }
    public override void ActionOnAnimFrame(EnemyController enemy)
    {
        enemy.SignalHub.OnPlayRandomSFX?.Invoke(abilitySFX, 0.075f);
        GameObject proj = Instantiate(projectile, spawnerTransform.SpawnTransform.position, Quaternion.identity);
        proj.GetComponent<BaseProjectile>().SetProjectileCourseToTarget(enemy.PlayerGO);
    }

    public override void EndAbility(EnemyController enemy)
    {
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, false);
    }

   
}
