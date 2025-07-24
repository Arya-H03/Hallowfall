using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class CSFX : MonoBehaviour
{
    [SerializeField] private AudioClip [] audioClip;
    [SerializeField] private int soundVolume = 1;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound(Vector3 audioSourceSpawnPos)
    {
        AudioManager.Instance.PlaySFX(audioClip, audioSourceSpawnPos, soundVolume);
    }

}
    