using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityHandler : MonoBehaviour
{
    
    [SerializeField] private List<PlayerBaseAbilitySO> playerAbilityPool = new();
    [SerializeField] private Dictionary<PlayerBaseAbilitySO, bool> playerAbilityDictionary = new();

    public Dictionary<PlayerBaseAbilitySO, bool> PlayerAbilityDictionary => playerAbilityDictionary;
    public List<PlayerBaseAbilitySO> PlayerAbilityPool => playerAbilityPool;
  
    public void UnlockAbility(PlayerBaseAbilitySO playerAbilityBaseData)
    {
        playerAbilityBaseData.TriggerAbility();

        if (!PlayerAbilityDictionary.ContainsKey(playerAbilityBaseData))
        {
            PlayerAbilityDictionary.Add(playerAbilityBaseData, true);
        }
        if (!playerAbilityBaseData.canLevelUp)
        {
            playerAbilityPool.Remove(playerAbilityBaseData);
        }
    }

    public void AddToAbilityPool(PlayerBaseAbilitySO playerAbilityBaseData)
    {
        playerAbilityPool.Add(playerAbilityBaseData);
    }

    public void RemoveFromAbilityPool(PlayerBaseAbilitySO playerAbilityBaseData)
    {
        playerAbilityPool.Remove(playerAbilityBaseData);
    }
}
