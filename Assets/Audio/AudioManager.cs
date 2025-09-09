using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [SerializeField] private ObjectPool audioPool;
    [SerializeField] private AudioSource audioSourcePrefab;

    private float masterVolumeLevel;
    private float sfxVolumeLevel;
    private float musicVolumeLevel;

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

    public float MasterVolumeLevel { get => masterVolumeLevel; set => masterVolumeLevel = value; }
    public float SFXVolumeLevel { get => sfxVolumeLevel; set => sfxVolumeLevel = value; }
    public float MusicVolumeLevel { get => musicVolumeLevel; set => musicVolumeLevel = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;


        MyUtils.ValidateFields(this, audioMixer, nameof(audioMixer));
        MyUtils.ValidateFields(this, sfxMixerGroup, nameof(sfxMixerGroup));
        MyUtils.ValidateFields(this, musicMixerGroup, nameof(musicMixerGroup));
        MyUtils.ValidateFields(this, audioPool, nameof(audioPool));

    }

    public void SetMasterVolume(float level)
    {
        float value = Mathf.Log10(level) * 20f;
        audioMixer.SetFloat("MasterVolume", value);
        masterVolumeLevel = level;
    }

    public void SetSFXVolume(float level)
    {
        float value = Mathf.Log10(level) * 20f;
        audioMixer.SetFloat("SFXVolume", value);
        sfxVolumeLevel = level;
    }

    public void SetMusicVolume(float level)
    {
        float value = Mathf.Log10(level) * 20f;
        audioMixer.SetFloat("MusicVolume", value);
        musicVolumeLevel = level;
    }

    public void PlaySFX(AudioClip audioClip, Vector3 spawnPos, float volume)
    {    
        AudioSource audioSource = audioPool.GetFromPool(spawnPos,Quaternion.identity,null).GetComponent<AudioSource>();

        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.pitch = Random.Range(0.9f, 1.00f);
        audioSource.outputAudioMixerGroup = sfxMixerGroup;

        audioSource.Play();

        float audioClipDuration = audioSource.clip.length;

        audioPool.ReturnToPool(audioSource.gameObject, audioClipDuration);
    }

    public void PlaySFX(AudioClip[] audioClips, Vector3 spawnPos, float volume)
    {
        AudioSource audioSource = audioPool.GetFromPool(spawnPos, Quaternion.identity, null).GetComponent<AudioSource>();

        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];

        audioSource.volume = volume;
        audioSource.pitch = Random.Range(0.9f, 1.00f);
        audioSource.outputAudioMixerGroup = sfxMixerGroup;

        audioSource.Play();

        float audioClipDuration = audioSource.clip.length;

        audioPool.ReturnToPool(audioSource.gameObject, audioClipDuration);
    }

    public void PlayMusic(AudioClip audioClip, Vector3 spawnPos, float volume)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnPos, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = musicMixerGroup;

        audioSource.loop = true;

        audioSource.Play();


    }

    public void PlayMusic(AudioClip[] audioClips, Vector3 spawnPos, float volume)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnPos, Quaternion.identity);

        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = musicMixerGroup;

        audioSource.loop = true;

        audioSource.Play();


    }
    public void StopAudioSource(AudioSource source)
    {
        source.Stop();
    }

    public void SaveSoundData()
    {
        SaveSystem.UpdateAudioSettings(this);
    }

    public void LoadSoundData()
    {
        SettingsData soundData = SaveSystem.LoadSettingsData();
        if (soundData != null)
        {
            MasterVolumeLevel = soundData.masterVolume;
            masterVolumeSlider.value = MasterVolumeLevel;
            SetMasterVolume(MasterVolumeLevel);

            MusicVolumeLevel = soundData.musicVolume;
            musicVolumeSlider.value = MusicVolumeLevel;
            SetMusicVolume(MusicVolumeLevel);

            SFXVolumeLevel = soundData.effectsVolume;
            effectsVolumeSlider.value = SFXVolumeLevel;
            SetSFXVolume(SFXVolumeLevel);
        }
    }
}
