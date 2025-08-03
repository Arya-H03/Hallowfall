using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will hold the data related to the playerGO

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "PlayerGO Config Data")]
public class PlayerConfig : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100;
    public int toLevel = 3;
    public float damageModifier = 1;

    [Header("Run State")]
    public float runSpeed = 3.5f;
    public AudioClip[] groundSFX;
    public AudioClip[] stoneSFX;
    public AudioClip[] grassSFX;

    [Header("Attack State")]

    public GameObject firstSwingEffect;
    public GameObject secondSwingEffect;
    public GameObject hitEffect;

    public float moveSpeedWhileAttaking = 2;
    public float hitStopDuration = 0.05f;
    public float swingComboWindow = 0.55f;
    public float delayBetweenSwings = 0.35f;
    public float swingInputLifeTime = 0.4f;
    public AudioClip[] attackSwingSFX;
    public float firstSwingDamage = 10;
    public float secondSwingDamage = 20;
    public float thirdSwingDamage = 30;
    public LayerMask enemyMask; 
    public float distance = 0; 
    public Vector2 firstSwingCastSize = new (1.7f, 1.5f);
    public Vector2 secondSwingCastSize = new (1.3f, 1f);
    public Vector2 thirdSwingCastSize = new (1.2f, 2f);

    [Header("Dash State")]
    public float dashModifier = 6;
    public float dashAttackDelay = 2f;
    public AudioClip[] dashAttackSFX;
    public float dashAttackDamage = 20;
    public float dashduration = 0.5f;

    [Header("Roll State")]
    public AudioClip rollSFX;
    public float rollCooldown = 1;
    public float rollModifier = 6;
    public float rollDuration = 0.5f;

    [Header("Parry State")]
    public GameObject impactEffectPrefab;
    public AudioClip parrySFX;
    public float parryWindow = 0.3f;

    [Header("VFX")]
    public GameObject afterImagePefab;
    public float afterImageLifeTime = 0.35f;
}
