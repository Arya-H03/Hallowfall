using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmiteHandler : ActiveAbilityHandler
{
    [SerializeField] SpawnerAbility smiteSpawnerSO;
    private int currentSpawnCount;
    private float currentSpawnDelay;


    [SerializeField] PassiveAbility quickenedSmite;
    [SerializeField] PassiveAbility echoingSmite;

   
   
    private void Start()
    {
        InitializePassiveAbility(quickenedSmite, QuickenedSmite);
        InitializePassiveAbility(echoingSmite, EchoingSmite);

        currentSpawnCount = smiteSpawnerSO.spawnCount;
        currentSpawnDelay = smiteSpawnerSO.spawnDelay;

        StartCoroutine(SpawnSmiteCoroutine());
    }

    private IEnumerator SpawnSmiteCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnDelay);

            for (int i = 0; i < currentSpawnCount; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(transform.position.x - 3, transform.position.x + 4), Random.Range(transform.position.y - 3, transform.position.y + 4), 0);
                GameObject effect = Instantiate(smiteSpawnerSO.projectile, spawnPos, Quaternion.identity);
                AudioManager.Instance.PlayRandomSFX(audioSource, sfx);
            }

        }

    }

    //Support Abilities

    private void EchoingSmite()
    {
        currentSpawnCount += (int)echoingSmite.modifier;
        
    }

    private void QuickenedSmite()
    {
        currentSpawnDelay *= (1 - quickenedSmite.modifier);
        
    }
}
