using System;
using UnityEngine;

public class EnemySignalHub
{
    
    public Action<float,float> OnEnemyHealthChange;
    public Action<Vector2,float> OnEnemyKnockBack;

    public Action<int> OnEnemyTurn;

    public Action<BaseEnemyAbilitySO> OnAbilityStart;
    public Action <EnemyController> OnAbilityFinished;
    public Action <EnemyController> OnAbilityAnimFrame;
    public Func<string,float> RequestAnimLength;

    public Action<AudioClip, float> OnPlaySFX;
    public Action<AudioClip[], float> OnPlayRandomSFX;
    public Action<HitSfxType, float> OnPlayHitSFX;

    public Action<string, bool> OnAnimBool;
    public Action<string> OnAnimTrigger;
    public Action<string> OnResetAnimTrigger;

    public Action OnItemDrop;

    public Action OnActivateHealthbar;
    public Action OnDeActivateHealthbar;

    public Action OnDisablePhysicsAndCollision;
    public Action OnEnablePhysicsAndCollision;

    public Action<int> OnRestoreHealth;
    public Action OnRestoreFullHealth;

    public Action OnPlayBloodEffect;

    public Action OnEnemyDeathBegin;
    public Action OnEnemyDeathEnd;
}
