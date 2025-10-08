using UnityEngine;
public abstract class BaseSkillSO : ScriptableObject
{
    public string abilityName;
    public string description;
    public Sprite icon;

    public abstract void Init(PlayerController controller);
    public abstract string GetDescription();
    
}
