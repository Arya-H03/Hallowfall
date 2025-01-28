using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private float masterVolumeMultiplier = 0.5f;
    [SerializeField] private float effectsVolumeMultiplier = 0.5f;
    [SerializeField] private float musicVolumeMultiplier = 0.5f;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
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

    public float MasterVolumeMultiplier { get => masterVolumeMultiplier; set => masterVolumeMultiplier = value; }
    public float EffectsVolumeMultiplier { get => effectsVolumeMultiplier; set => effectsVolumeMultiplier = value; }
    public float MusicVolumeMultiplier { get => musicVolumeMultiplier; set => musicVolumeMultiplier = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;


        

    }

    public void PlaySFX(AudioSource source, AudioClip clip)
    {
        source.volume =  MasterVolumeMultiplier * EffectsVolumeMultiplier;
        source.PlayOneShot(clip);
    }

    public void PlayMusic(AudioSource source, AudioClip clip)
    {
        source.volume =  MasterVolumeMultiplier * MusicVolumeMultiplier;
        source.PlayOneShot(clip);
    }

    public void StopAudioSource(AudioSource source)
    {
        source.Stop();  
    }

    public void SaveSoundData()
    {
        SaveSystem.SaveSoundData(this);
    }

    public void LoadSoundData()
    {
        SoundData soundData = SaveSystem.LoadSoundData();
        if (soundData != null)
        {
            MasterVolumeMultiplier = soundData.masterVolume;
            masterVolumeSlider.value = MasterVolumeMultiplier;

            MusicVolumeMultiplier = soundData.musicVolume;
            musicVolumeSlider.value = MusicVolumeMultiplier;

            EffectsVolumeMultiplier = soundData.effectsVolume;
            effectsVolumeSlider.value = EffectsVolumeMultiplier;
        }
    }
}
