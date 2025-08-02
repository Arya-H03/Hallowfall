using UnityEngine;
public class PlayerHasteAbility : MonoBehaviour,IAbility
{
    private PlayerController playerController;
    [SerializeField] float speedModifier = 0.1f;

    private CSFX cSFX;

    private void Awake()
    {
        Init();      
    }
    public void PassPlayerControllerRef(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    public void Init()
    {
        cSFX = GetComponentInChildren<CSFX>();
        MyUtils.ValidateFields(this, cSFX, "CSFX");
    }

    public void Perform()
    {     
        //playerController.StateMachine.PlayerRunState.RunSpeed *= 1 + speedModifier;
        //cSFX.PlaySound(playerController.transform.position);
    }
}
