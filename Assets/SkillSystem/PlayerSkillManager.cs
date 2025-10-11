using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private static PlayerSkillManager instance;

    public static PlayerSkillManager Instance
    {
        get { return instance; }
    }

    [SerializeField] private List<BaseSkillSO> totalAvailablePlayerSkills;
    [SerializeField] private List<BaseSkillSO> unlockedPlayerSkills;
    [SerializeField] private List<BaseSkillSO> lockedPlayerSkills;
    private PlayerController playerController;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        unlockedPlayerSkills = new List<BaseSkillSO>();
        lockedPlayerSkills = new List<BaseSkillSO>(totalAvailablePlayerSkills);
    }

    private void Start()
    {
        playerController = GameManager.Instance.PlayerController;
    }
    public BaseSkillSO[] GetThreeRandomSkills()
    {
        BaseSkillSO[] skills = new BaseSkillSO[3];
        List<BaseSkillSO> temp = new List<BaseSkillSO>(lockedPlayerSkills);
        skills[0] = MyUtils.GetRandomRef<BaseSkillSO>(temp);
        temp.Remove(skills[0]);
        skills[1] = MyUtils.GetRandomRef<BaseSkillSO>(temp);
        temp.Remove(skills[1]);
        skills[2] = MyUtils.GetRandomRef<BaseSkillSO>(temp);
        
        return skills;
    }

    public void UnlockPlayerSkill(BaseSkillSO skill) 
    {
        lockedPlayerSkills.Remove(skill);
        unlockedPlayerSkills.Add(skill);
        skill.Init(playerController);
    }
}
