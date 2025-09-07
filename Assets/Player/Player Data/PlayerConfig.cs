using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will hold the data related to the playerGO

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerGO Config Data")]
public class PlayerConfig : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int toLevel = 3;
    public float damageModifier = 1;

    [Header("Run State")]
    public float runSpeed = 3.5f;
    public AudioClip[] groundSFX;
    public AudioClip[] stoneSFX;
    public AudioClip[] grassSFX;

    [Header("Attack1 State")]

    public List<ComboAttackConfigStruct> comboAttacks;
    public GameObject firstSwingEffect;
    public GameObject secondSwingEffect;
    public GameObject thirdSwingEffect;
    public GameObject hitEffect;

    public float moveSpeedWhileAttaking = 2;
    public float hitStopDuration = 0.05f;
    public float swingComboWindow = 0.55f;
    public AudioClip[] attackSwingSFX;
    public LayerMask enemyMask; 


    [Header("Dash State")]
    public float dashModifier = 6;
    public float dashAttackDelay = 2f;
    public AudioClip[] dashAttackSFX;
    public int dashAttackDamage = 20;
    public float dashduration = 0.5f;

    [Header("Roll State")]
    public AudioClip rollSFX;
    public float rollCooldown = 1;
    public float rollModifier = 6;
    public float rollDuration = 0.5f;

    [Header("Parry State")]
    public GameObject impactEffectPrefab;
    public AudioClip [] parrySFX;
    public float parryWindow = 0.3f;

    [Header("VFX")]
    public GameObject afterImagePefab;
    public float afterImageLifeTime = 0.35f;

    [Header("Hit Handler")]
    public float cameraShakeOnHitDuration = 0.3f;
    public float cameraShakeOnHitIntensity = 0.05f;

    public float vignetteFlashOnHitIntensity = 0.5f;
    public Color vignetteFlashOnHitColor = Color.darkRed;

    public AudioClip hitSFX;
}
