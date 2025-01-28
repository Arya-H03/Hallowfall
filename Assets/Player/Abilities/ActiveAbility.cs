using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActiveAbility", menuName = "NewActiveAbility")]
public class ActiveAbility : BaseAbility
{
    public GameObject projectile;
    public GameObject spawnerGO;
    public int spawnCount = 1;
    public float spawnDelay = 2;

    public System.Type spawnerType;

    public PassiveAbility[] supportAbilities; 

    public override void ApplyAbility()
    {
        GameObject spawner = Instantiate(spawnerGO, GameManager.Instance.Player.transform.Find("AbilityHolder"));
        ProjectileSpawner projectileSpawner = spawner.GetComponent<ProjectileSpawner>();
        projectileSpawner.Ability = this;
        
        foreach (PassiveAbility ability in supportAbilities)
        {
            LevelupManager.Instance.abilities.Add(ability);
        }
      
        LevelupManager.Instance.abilities.Remove(this);
}
}
