using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill Scriptable Object",menuName = " Skill SO")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Sprite icon;
    public int skillCost;

    public void ApplySkill()
    {
        Debug.Log(skillName + " has been applied");
    }
}
