using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{

    [SerializeField] GameObject skillDescriptionFrame;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI resetCostText;

    [SerializeField] TextMeshProUGUI skullText;

    private SkillNode[] skillNodes;

    [SerializeField] private int resetCost = 100;

    public SkillNode[] SkillNodes { get => skillNodes;}

    private void Awake()
    {
        skillNodes = GetComponentsInChildren<SkillNode>();
    }
    private void Start()
    {
        resetCostText.text = resetCost.ToString();
       
    }
    public void ShowDescriptionFrame(Vector3 pos,string name,string description,int cost)
    {
  
        skillDescriptionFrame.transform.position = pos + new Vector3(130,70,0);
        nameText.text = name;
        descriptionText.text = description;
        costText.text = cost.ToString();

        skillDescriptionFrame.SetActive(true);
    }

    public void HideDescriptionFrame()
    {
        skillDescriptionFrame.SetActive(false);
    }

    public void ResetAllSkills()
    {
        int temp = LoadSkullCount() - resetCost;
        if (temp >= 0)
        {
            foreach (SkillNode skillNode in SkillNodes)
            {
                skillNode.ResetSkill();
            }
            SaveSystem.UpdatePlayerSkulls(temp);
            UpdateSkullsText();
        }
       
    }

    public int LoadSkullCount()
    {
        GameData gameData = SaveSystem.LoadGameData();
        if (gameData != null)
        {
            return gameData.skullCount;
        }
        else return 0;
    }

  
    public void UpdateSkullsText()
    {
        skullText.text = LoadSkullCount().ToString();
    }

    public void ApplySavedSkillTree(int[] savedData)
    {
        foreach (var node in SkillNodes)
        {
            node.UnlockBasedOfSkillTreeData(savedData);
        }
    }


}
