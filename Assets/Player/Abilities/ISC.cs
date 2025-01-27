using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ISC_Ability", menuName = "ISC_Ability")]
public class ISC : PassiveAbility //Increase Spawn Count
{
    public ActiveAbility activeAbility;
    public int modifier; //Should be int


    public override void ApplyAbility()
    {
        ProjectileSpawner spawner = GameManager.Instance.Player.transform.Find("AbilityHolder").gameObject.GetComponentInChildren<ProjectileSpawner>();
        if (spawner)
        {
            spawner.CurrentSpawnCount += modifier;

        }
        else
        {
            Debug.LogWarning($"{this.abilityName} couldn't the spawner");
        }

    }

}
