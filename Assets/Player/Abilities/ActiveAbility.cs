using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveAbility", menuName = "ActiveAbility")]
public class ActiveAbility : BaseAbility
{  
    public GameObject handlerPrefab;
   
    public PassiveAbility[] supportAbilities; 

    public override void CallAbility()
    {
        

        GameObject handlerGO = Instantiate(handlerPrefab, GameManager.Instance.Player.transform.Find("AbilityHolder"));
        ActiveAbilityHandler handler = handlerGO.GetComponent<ActiveAbilityHandler>();
        Debug.Log("yo");


        foreach (PassiveAbility ability in supportAbilities)
        {
            LevelupManager.Instance.abilities.Add(ability);
        }

        LevelupManager.Instance.abilities.Remove(this);

        base.CallAbility();
    }
}
