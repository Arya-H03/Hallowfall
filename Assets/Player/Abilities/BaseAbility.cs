using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BaseAbility : ScriptableObject
{
    public string abilityName;
    public string description;
    public Sprite icon;
    public bool canLevel;

    public virtual void CallAbility() 
    {
        UIManager.Instance.AddAbilitySlot(this);
    }

    
}
