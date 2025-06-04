using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowStrikeHandler : ActiveAbilityHandler
{
    public List<GameObject> nearbyEnemies = new List<GameObject>();

    [SerializeField] SpawnerAbility shadowStrikeSpawnerSO;
    [SerializeField] private LayerMask enemyLayer;

    private int currentSpawnCount;
    private float currentSpawnDelay;

    private void Start()
    {
        currentSpawnCount = shadowStrikeSpawnerSO.spawnCount;
        currentSpawnDelay = shadowStrikeSpawnerSO.spawnDelay;

        StartCoroutine(SpawnShadowCloneCoroutine());
    }

    private IEnumerator SpawnShadowCloneCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnDelay);

            RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, 10f, Vector2.zero, 0, enemyLayer);
            foreach (RaycastHit2D enemy in enemies)
            {
                if (enemy.collider.CompareTag("Enemy"))
                {
                    nearbyEnemies.Add(enemy.transform.gameObject);
                }
            }

            if (nearbyEnemies.Count > 0)
            {
                SummonShadowClone();
            }
           
        }

    }
    private void SummonShadowClone()
    {
        // Filter out already marked enemies
        if (nearbyEnemies.Count > 0)
        {
            int index = Random.Range(0, nearbyEnemies.Count);
            GameObject selectedEnemy = nearbyEnemies[index];
            GameObject clone = Instantiate(shadowStrikeSpawnerSO.projectile, selectedEnemy.transform.transform.position, Quaternion.identity);
            clone.GetComponent<ShadowClone>().EnemyTarget = selectedEnemy;
           

        }
    }
}
