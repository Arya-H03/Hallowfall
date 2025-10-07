using UnityEngine;
public class BaseSkillSO : ScriptableObject
{
    public string abilityName;
    public string description;
    public Sprite icon;

    public virtual void Init(EntityController controller) { }
    public virtual string GetDescription() { return ""; }
    
}
