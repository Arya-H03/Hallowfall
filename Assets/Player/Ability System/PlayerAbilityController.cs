using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    [SerializeField] private List<PlayerAbilityData> playerAbilityPool = new List<PlayerAbilityData>();
    [SerializeField] private List<PlayerAbilityData> unlockedPlayerAbilities  = new List<PlayerAbilityData>();
    [SerializeField] private List<PlayerAbilityData> availablePlayerAbilities = new List<PlayerAbilityData>();

    public List<PlayerAbilityData> AvailablePlayerAbilities => availablePlayerAbilities;
    private void Start()
    {
        RefillLockedPlayerAbilities();
    }

    public void RefillLockedPlayerAbilities()
    {

        availablePlayerAbilities = new List<PlayerAbilityData>(playerAbilityPool);
    }

    public void UnlockAbility(PlayerAbilityData playerAbilityData)
    {
        if(!unlockedPlayerAbilities.Contains(playerAbilityData))unlockedPlayerAbilities.Add(playerAbilityData);
        
        availablePlayerAbilities.Remove(playerAbilityData); 
        if(!playerAbilityData.canLevelUp) playerAbilityPool.Remove(playerAbilityData);
    }
}
