using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] private PlayerAbilitySO playerAbilityData;
    private List<IAbilityComponent> abilityComponentsList;
    public PlayerAbilitySO PlayerAbilityData { get => playerAbilityData;}

    private void OnValidate()
    {
        MyUtils.ValidateFields(this,playerAbilityData,nameof(playerAbilityData));
    }

    private void Awake()
    {
        abilityComponentsList = new List<IAbilityComponent>(GetComponents<IAbilityComponent>());
        if (abilityComponentsList.Count == 0) Debug.LogWarning($"No Ability Components are attached to {this.gameObject.name}");
    }


    public void ExecuteAbility()
    {
        foreach(IAbilityComponent abilityComponent in abilityComponentsList)
        {
            abilityComponent.Perform();
        }
    }

    
}
