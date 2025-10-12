using UnityEngine;
public abstract class BaseSkillSO : ScriptableObject
{
    [SerializeField] private string abilityName;
    protected string skillDescription;
    public Sprite icon;
    public int lvl = 0;

    public void Init(PlayerController playerController)
    {
        if(!playerController || lvl < 0) return;

        UIManager.Instance.AddToSkillFrame(this);

        if (lvl == 0) ApplySkillLogic(playerController);
        else LevelUpSkill(playerController);

       
    }

    public string GetSkillName()
    {
        return abilityName + $"<color=yellow> {MyUtils.ToRomanNumeral(lvl)}</color>";
    }


    public abstract void ApplySkillLogic(PlayerController playerController);
    public abstract void LevelUpSkill(PlayerController playerController);
    public abstract string GetSkillDescription();
   
    
}
