using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmiteSpawner : ProjectileSpawner
{
  
    protected override IEnumerator SpawnEffectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnDelay);

            for (int i = 0; i < SpawnCount; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(transform.position.x - 3, transform.position.x + 4), -3.5f, 0);
                GameObject effect = Instantiate(Projectile, spawnPos, Quaternion.identity);
            }
           
        }



    }
}
