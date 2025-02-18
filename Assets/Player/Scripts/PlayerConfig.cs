using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will hold the data related to the player

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player Config Data")]
public class PlayerConfig : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100;
    public int toLevel = 3;
    public float runSpeed = 3.5f;


    [Header("SFX")]
    public AudioClip[] groundSFX; 
    public AudioClip[] stoneSFX;
    public AudioClip[] grassSFX;


}
