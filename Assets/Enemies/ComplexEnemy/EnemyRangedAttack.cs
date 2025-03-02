using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttack : EnemyBaseAttack
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPoint;

    public override void CallAttackActionOnAnimFrame()
    {
        SpawnProjectile();
        PlayAttackSFX();

    }

    private void SpawnProjectile()
    {
        GameObject projGO = Instantiate(projectilePrefab, projectileSpawnPoint.position,Quaternion.identity);
        BaseProjectile projectile = projGO.GetComponent<BaseProjectile>();
        projectile.Damage = attackDamage;
        projectile.SetProjectileCourseToTarget(enemyController.Player.transform);
    }
}
