using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public enum HitSfxType
{
    none,
    sword,
    fire
}
[System.Serializable]
public struct HitSFX
{
    public HitSfxType hitType;
    public AudioClip[] clips;
}

public class EnemySFXHandler : MonoBehaviour, IInitializeable<EnemyController>
{

    private AudioManager audioManager;
    private EnemySignalHub signalHub;
    private EnemyController enemyController;

    private Dictionary<HitSfxType, AudioClip[]> hitSFXDict = new();

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        signalHub = enemyController.SignalHub;

        audioManager = AudioManager.Instance;
        signalHub.OnPlayRandomSFX += PlayRandomSFX;
        signalHub.OnPlaySFX += PlaySFX;
        signalHub.OnPlayHitSFX += PlayHitSFX;

        FillDictionary(); 
    }

    private void OnDisable()
    {
        if(signalHub == null) return;
        signalHub.OnPlayRandomSFX -= PlayRandomSFX;
        signalHub.OnPlaySFX -= PlaySFX;
        signalHub.OnPlayHitSFX = PlayHitSFX;
    }
    
    private void FillDictionary()
    {
        foreach (HitSFX hitSFX in enemyController.EnemyConfig.hitSFXList)
        {
            if (!hitSFXDict.ContainsKey(hitSFX.hitType)) hitSFXDict.Add(hitSFX.hitType, hitSFX.clips);

        }
    }
    private void PlayRandomSFX(AudioClip[] audioClip, float volume)
    {
        audioManager.PlaySFX(audioClip, enemyController.GetEnemyPos(), volume);
    }

    private void PlayHitSFX(HitSfxType hitType, float volume)
    {
        audioManager.PlaySFX(hitSFXDict[hitType], enemyController.GetEnemyPos(), volume);
    }

    private void PlaySFX(AudioClip audioClip,float volume)
    {
        audioManager.PlaySFX(audioClip, enemyController.GetEnemyPos(), volume);
    }
}
