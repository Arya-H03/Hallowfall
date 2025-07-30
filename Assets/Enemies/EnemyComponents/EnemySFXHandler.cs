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

public class EnemySFXHandler : MonoBehaviour, IInitializeable<EnemyController>
{
    [System.Serializable]
    struct HitSFX
    {
        public HitSfxType hitType;
        public AudioClip[] sound;
    }

    AudioManager audioManager;
    EnemySignalHub signalHub;
    EnemyController enemyController;

    [SerializeField] HitSFX[] hitSFXs;
    private Dictionary<HitSfxType, AudioClip[]> hitSFXDictionary;

    private Action<float, HitSfxType> cachedSFXHitHandler;
    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        signalHub = enemyController.SignalHub;

        audioManager = AudioManager.Instance;

        cachedSFXHitHandler = (damageAmount, hitSfxType) => { PlayHitSFX(hitSfxType); };
        signalHub.OnEnemyHit += cachedSFXHitHandler;
    }

    private void OnDisable()
    {
        if(signalHub == null) return;
        signalHub.OnEnemyHit -= cachedSFXHitHandler;
    }
    private void Start()
    {
        FillDictionary();
    }
    private void FillDictionary()
    {
        hitSFXDictionary = new Dictionary<HitSfxType, AudioClip[]>();
        foreach (var hitSfx in hitSFXs)
        {
            hitSFXDictionary[hitSfx.hitType] = hitSfx.sound;
        }
    }
    private AudioClip GetHitSound(HitSfxType hitType)
    {
        if (hitSFXDictionary.TryGetValue(hitType, out AudioClip[] sfx))
        {
            return sfx.Length > 0 ? sfx[Random.Range(0, sfx.Length)] : null;

        }
        return null;
    }

    private void PlayHitSFX(HitSfxType hitType)
    {
        audioManager.PlaySFX(GetHitSound(hitType), enemyController.GetEnemyPos(), 0.4f);
    }

    public void PlaySFX(AudioClip audioClip,float volume)
    {
        audioManager.PlaySFX(audioClip, enemyController.GetEnemyPos(), volume);
    }
}
