using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSO : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite icon;
    public int skillCost;
    public int id;

    public virtual void ApplySkill(PlayerController playerController)
    {
       
    }
}
