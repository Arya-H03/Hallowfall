using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public float masterVolume = 1;
    public float effectsVolume = 1;
    public float musicVolume = 1;

    public SoundData(AudioManager audioManager)
    {
        masterVolume = audioManager.MasterVolumeMultiplier;
        effectsVolume = audioManager.EffectsVolumeMultiplier;
        musicVolume = audioManager.MusicVolumeMultiplier;
    }
}
