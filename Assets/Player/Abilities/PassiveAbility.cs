using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveAbility", menuName = "PassiveAbility")]
public class PassiveAbility : BaseAbility
{
    public float modifier;
    public delegate void PassiveAbilityEvent();
    public event PassiveAbilityEvent passiveAbilityEvent;

    public override void CallAbility()
    {
        passiveAbilityEvent?.Invoke();
    }
}
