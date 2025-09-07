using UnityEngine;

public class PlayerVitalityAbility : MonoBehaviour,IAbility
{
    private PlayerController playerController;
    public PlayerAbilitySO AbilitySO { get; set; }
    [SerializeField] int healthModifier = 25;
    public void InjectReferences(PlayerController playerController, PlayerAbilitySO abilitySO)
    {
        this.playerController = playerController;
    }
    public void Init()
    {
    }

    public void Perform()
    {
        playerController.PlayerSignalHub.MaxHealthBinding.Setter(playerController.PlayerSignalHub.MaxHealthBinding.Getter() + healthModifier);
        playerController.PlayerSignalHub.OnRestoreHealth?.Invoke(healthModifier);
    }
}
