using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpecialAbility : EnemyBaseAttack
{
    [SerializeField] GameObject mob;

    public override void HandleAttack()
    {
        SpawnMob(FindSpawnPos());
    }

    private Vector3  FindSpawnPos()
    {
        Vector3 spawnPos = enemyController.transform.position;
        float offset = Random.Range(1, 3);
        spawnPos += enemyController.EnemyMovement.CurrentDir * Vector3.right * offset;

        return spawnPos;
    }
    private void SpawnMob(Vector3 spawnPos)
    {
        Instantiate(mob,spawnPos,Quaternion.identity);  
    }
}
