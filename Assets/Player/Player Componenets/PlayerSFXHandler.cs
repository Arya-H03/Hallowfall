using UnityEngine;

public class PlayerSFXHandler : MonoBehaviour, IInitializeable<PlayerController>
{
    private AudioManager audioManager;
    private PlayerController playerController;

    private AudioClip[] groundRunSFX;
    private AudioClip[] grassRunSFX;
    private AudioClip[] stoneRunSFX;
    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        audioManager = AudioManager.Instance;

        groundRunSFX = playerController.PlayerConfig.groundSFX;
        grassRunSFX = playerController.PlayerConfig.grassSFX;
        stoneRunSFX = playerController.PlayerConfig.stoneSFX;

        playerController.PlayerSignalHub.OnPlayerStep += PlayStepSound;
        playerController.PlayerSignalHub.OnPlaySFX += PlaySFX;
        playerController.PlayerSignalHub.OnPlayRandomSFX += PlayRandomSFX;
    }

    private void OnDisable()
    {
        playerController.PlayerSignalHub.OnPlayerStep -= PlayStepSound;
        playerController.PlayerSignalHub.OnPlaySFX -= PlaySFX;
        playerController.PlayerSignalHub.OnPlayRandomSFX -= PlayRandomSFX;
    }

    public void PlaySFX(AudioClip audioClip, float volume)
    {
        audioManager.PlaySFX(audioClip, playerController.GetPlayerPos(), volume);
    }

    public void PlayRandomSFX(AudioClip[] audioClip, float volume)
    {
        audioManager.PlaySFX(audioClip, playerController.GetPlayerPos(), volume);
    }
    public void PlayStepSound()
    {

        switch (playerController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                PlayRandomSFX(groundRunSFX, 0.25f);
                break;
            case FloorTypeEnum.Grass:
                PlayRandomSFX(grassRunSFX, 0.25f);
                break;
            case FloorTypeEnum.Stone:
                PlayRandomSFX(stoneRunSFX, 0.25f);
                break;
        }
    }
}
