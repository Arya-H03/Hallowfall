using UnityEngine;
[CreateAssetMenu(fileName = "PlayerAbilityData", menuName = "Scriptable Objects/PlayerAbilityData")]
public class PlayerAbilityData : ScriptableObject
{
    public string abilityName;
    public string ailityDescription;
    public Sprite abilityIcon;

    public bool canLevelUp;

    public GameObject abilityPrefab;
    private IAbility abilitRef;
    private PlayerController playerController;

    public void OnAbilityUnlocked()
    {
       
        if (abilitRef == null)
        {
            playerController = GameManager.Instance.PlayerController;
            GameObject abilityGO = Instantiate(abilityPrefab);           
            abilityGO.transform.parent = playerController.PlayerAbilityController.gameObject.transform;
            abilityGO.transform.position = Vector3.zero;    
            abilityGO.transform.rotation = Quaternion.identity;

            abilitRef = abilityGO.GetComponent<IAbility>();
        }
        else
        {
            abilitRef.PassPlayerControllerRef(playerController);
            abilitRef.Perfom();
        }


           
    }
}
