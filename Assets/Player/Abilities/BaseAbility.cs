using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BaseAbility : ScriptableObject
{
    public string abilityName;
    public string description;
    public Sprite icon;

    public virtual void ApplyAbility() 
    {
        
    }
}
