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
        playerController.MaxHealth += 25;
        playerController.RestoreHealth(playerController.MaxHealth);
    }
}
