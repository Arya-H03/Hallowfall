using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RST_Ability", menuName = "RST_Ability")]
public class RST : PassiveAbility //Reduce Spawn Time 
{
    public ActiveAbility activeAbility;
    public float modifier; //Should be %


    public override void ApplyAbility()
    {
        ProjectileSpawner spawner = GameManager.Instance.Player.transform.Find("AbilityHolder").gameObject.GetComponentInChildren<ProjectileSpawner>();
        if (spawner)
        {
            spawner.CurrentSpawnDelay *= (1 - modifier);
           
        }
        else
        {
            Debug.LogWarning($"{this.abilityName} couldn't the spawner");
        }
       
    }


}
