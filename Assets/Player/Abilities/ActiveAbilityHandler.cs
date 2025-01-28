using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityHandler : MonoBehaviour
{
    protected void InitializePassiveAbility(PassiveAbility passiveAbility, Action action)
    {
        passiveAbility.passiveAbilityEvent += () => action();
        
        
    }
}
