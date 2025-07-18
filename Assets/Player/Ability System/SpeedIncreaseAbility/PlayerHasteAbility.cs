using UnityEngine;
public class PlayerHasteAbility : MonoBehaviour,IAbility
{
    private PlayerController playerController;
    [SerializeField] float speedModifier = 0.1f;

    private CSFX cSFX;

    private void Awake()
    {
        Initialize();      
    }
    public void PassPlayerControllerRef(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    public void Initialize()
    {
        cSFX = GetComponentInChildren<CSFX>();
        MyUtils.ValidateFields(this, cSFX, "CSFX");
    }

    public void Perfom()
    {     
        playerController.PlayerRunState.RunSpeed *= 1 + speedModifier;
        cSFX.PlaySound();
    }
}
