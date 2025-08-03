using UnityEngine;

public class PlayerVitalityAbility : MonoBehaviour,IAbility
{
    private PlayerController playerController;
    [SerializeField] float healthModifier = 25f;
    public void PassPlayerControllerRef(PlayerController playerController)
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
