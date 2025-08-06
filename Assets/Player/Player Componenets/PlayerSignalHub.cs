using System;
using UnityEngine;

public class PropertyBinding<T>
{
    public Func<T> Getter { get; }
    public Action<T> Setter { get; }

    public PropertyBinding(Func<T> getter, Action<T> setter)
    {
        Getter = getter;
        Setter = setter;
    }
}
public class PlayerSignalHub
{
    //Hit Handler
    public Action<float> OnPlayerHit;
    public Action<float> OnPlayerDamage;
    public Action<float, float> OnPlayerHealthChange;
    public Action<float> OnRestoreHealth;
    public Action OnRestoreFullHealth;
    public PropertyBinding<float> MaxHealthBinding;

    public Action OnPlayerDeath;
    public Action OnPlayereDeSpawn;

    //Camera Handler

    public Action <float,float> OnCameraShake;
    public Action <float,float,Color> OnVignetteFlash;

    // Anim Handler
    public Action<string,bool> OnAnimBool;
    public Action<string> OnAnimTrigger;
    public Action OnPlayerStep;
    public Func<string, float> RequestAnimLength;

    //SFX Handler
    public Action<AudioClip,float> OnPlaySFX;
    public Action<AudioClip[],float> OnPlayRandomSFX;

    //Movement Handler
    public Action<float> OnChangeMoveSpeed;
    public Action<bool> OnAllowMovement;
    public Action<float> OnApplyForwardVelocity;
    public Action<Vector2,float> OnApplyDirectionVelocity;
    public Action OnResetVelocity;
    public Action <Vector2> OnTurning;
    public Action  OnTurningToMousePos;
    public Action<PlayerStateEnum> OnStateTransitionBasedOnMovement;
    public Action <float> OnChangeSpeedModifier;
    public PropertyBinding<Vector2> FacingDirctionBinding;

    //VFX Handler
    public Action OnAfterImageStart;
    public Action OnAfterImageStop;
    public Action <float>OnMaterialFlash;
    public Action<GameObject,Vector3,Quaternion,float> OnSpawnVFX;
    public Action<GameObject,Vector3,Quaternion,float,Vector3> OnSpawnScaledVFX;

    //State Machine
    public Action<PlayerStateEnum> OnChangeState;

    //SwordAttack
    public Action OnSwordAttackHitFrame;
    public Action OnSwordAttackSFXFrame;
    public Action OnSwordSwingEnd;

    //Parry
    public Action OnActivatingParryShield;
    public Action <EnemyController,float> OnEnemyParried;
    public Action OnParryAttackHit;
    public Action OnParryEnd;

    //Death
    public Action OnDeath;
}
