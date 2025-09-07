using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseAbilitySO : ScriptableObject
{
    public string abilityName;
    public string ailityDescription;
    public Sprite abilityIcon;

    public bool canLevelUp;

    //Reference to the class that handles the logic for the Ability
    protected IAbility abilityRef;

    public IAbility AbilityRef => abilityRef;


    //This gets called when the playerGO clicks on an Ability Card
    public abstract void TriggerAbility();

}

//For all of playerGO abilities

[CreateAssetMenu(fileName = "PlayerAbilitySO", menuName = "Scriptable Objects/PlayerAbilitySO")]
public class PlayerAbilitySO : PlayerBaseAbilitySO
{
    private PlayerController playerController;
    public GameObject abilityPrefab;
    public List<PlayerAbilityUpgradeSO> listOfAbilityUpgrades;

    public override void TriggerAbility()
    {
        playerController = GameManager.Instance.PlayerController;
        //First time an ability is selected
        if (!playerController.PlayerAbilityController.PlayerAbilityDictionary.ContainsKey(this))
        {
            GameObject abilityGO = Instantiate(abilityPrefab);
            abilityGO.transform.parent = playerController.PlayerAbilityController.gameObject.transform;
            abilityGO.transform.position = Vector3.zero;
            abilityGO.transform.rotation = Quaternion.identity;

            abilityRef = abilityGO.GetComponent<IAbility>();
            abilityRef.InjectReferences(playerController,this);

            abilityRef.Perform();

            foreach (PlayerAbilityUpgradeSO upgradeSO in listOfAbilityUpgrades)
            {
                upgradeSO.SetSourceAbility(this);
                playerController.PlayerAbilityController.AddToAbilityPool(upgradeSO);
            }
        }
        //When the ability is selected again
        else if (canLevelUp)
        {
            abilityRef.Perform();
        }



    }

}

//For all playerGO ability upgrades
public abstract class PlayerAbilityUpgradeSO : PlayerBaseAbilitySO, IAbilityUpgrade
{
    protected PlayerAbilitySO sourceAbility;

    public void SetSourceAbility(PlayerAbilitySO playerAbilityData)
    {
        sourceAbility = playerAbilityData; 
    }

    public override void TriggerAbility()
    {
        abilityRef = sourceAbility.AbilityRef;
        if(abilityRef is IUpgradeableAbility upgradeable)
        {
            upgradeable.ApplyUpgrade(this);    
        }
    }

    public abstract void ApplyUpgradeLogicTo(IAbility ability);

}

