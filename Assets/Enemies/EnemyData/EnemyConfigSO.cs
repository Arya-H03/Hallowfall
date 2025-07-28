using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will hold the data related to the playerGO

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/EnemyConfig")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("Stats Config")]
    public float maxHealth;
    public float damageModifier;
    public float maxStagger;
    public float timeBetweenStaggers;



    [Header("Chase State Config")]
    public float minChaseSpeed;
    public float maxChaseSpeed;

    [Header("Attack State Config")]
    public float minAttackDelay;
    public float maxAttackDelay;

    [Header("Stun State Config")]
    public float stunDuration;

    [Header("Death State Config")]
    public Sprite corpseSprite;
    public float corpseLifeTime;

    

}
