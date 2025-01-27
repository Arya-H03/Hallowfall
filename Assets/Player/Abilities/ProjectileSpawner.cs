using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner:MonoBehaviour
{
    private GameObject projectile;
    private int spawnCount;
    private float spawnDelay;
    public GameObject Projectile { get => projectile; set => projectile = value; }
    public int SpawnCount { get => spawnCount; set => spawnCount = value; }
    public float SpawnDelay { get => spawnDelay; set => spawnDelay = value; }

    private void Start()
    {
        StartCoroutine(SpawnEffectCoroutine());
    }

    protected virtual IEnumerator SpawnEffectCoroutine()
    {
       return null;
    }
}
    