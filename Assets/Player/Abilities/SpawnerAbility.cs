using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerAbility", menuName = "SpawnerAbility")]
public class SpawnerAbility : ActiveAbility
{
    public GameObject projectile;
    public int spawnCount = 1;
    public float spawnDelay = 2;


}
