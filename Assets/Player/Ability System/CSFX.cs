using UnityEngine;

public class CSFX : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private int soundModifier = 1;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound()
    {
        AudioManager.Instance.PlaySFX(audioSource, audioClip, soundModifier);
    }

}
