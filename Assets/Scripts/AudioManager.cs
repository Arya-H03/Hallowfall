using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private float overallVolumeMultiplier = 1;
    private float sfxVolumeMultiplier = 1;
    private float musicVolumeMultiplier = 1;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                instance = go.AddComponent<AudioManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;


        DontDestroyOnLoad(gameObject);

    }

    public void PlaySFX(AudioSource source, AudioClip clip)
    {
        source.volume = source.volume * overallVolumeMultiplier * sfxVolumeMultiplier;
        source.PlayOneShot(clip);
    }

    public void PlayMusic(AudioSource source, AudioClip clip)
    {
        source.volume = source.volume * overallVolumeMultiplier * musicVolumeMultiplier;
        source.PlayOneShot(clip);
    }

    public void StopAudioSource(AudioSource source)
    {
        source.Stop();  
    }
}
