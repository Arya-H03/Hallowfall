using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityHandler : MonoBehaviour
{
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] sfx;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected void InitializePassiveAbility(PassiveAbility passiveAbility, Action action)
    {
        passiveAbility.passiveAbilityEvent += () => action();
        
        
    }
}
