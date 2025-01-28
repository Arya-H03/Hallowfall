using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner:MonoBehaviour
{
    protected ActiveAbility ability;
    private int currentSpawnCount;
    private float currentSpawnDelay;

    public ActiveAbility Ability { get => ability; set => ability = value; }
    public int CurrentSpawnCount { get => currentSpawnCount; set => currentSpawnCount = value; }
    public float CurrentSpawnDelay { get => currentSpawnDelay; set => currentSpawnDelay = value; }

    private void Start()
    {
        currentSpawnCount = ability.spawnCount;
        currentSpawnDelay = ability.spawnDelay;
        ability.spawnerType = GetType();

        StartCoroutine(SpawnEffectCoroutine());
    }

    protected virtual IEnumerator SpawnEffectCoroutine()
    {
       return null;
    }
}
    