using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [Header("Node Dependencies")]
    [SerializeField] private SkillNode[] previousNodes;
    [SerializeField] private SkillNode[] nextNodes;

    [SerializeField] private SkillSO skillSO;
    private SkillTreeManager skillManager;

    [Header("Visuals")]
    [SerializeField] private Image outlineImage;
    [SerializeField] private Transform linkHolder;
    [SerializeField] private Image skillImage;

    private List<Image> links = new List<Image>();

    private bool isUnlocked = false;
    private bool canBeUnlocked = false;

    public bool IsUnlocked { get => isUnlocked; private set => isUnlocked = value; }

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillTreeManager>();

        foreach (Transform child in linkHolder)
        {
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                links.Add(image);
            }
        }
    }

    private void Start()
    {
        CheckPreviousNodes();
        skillImage.sprite = skillSO.icon;
    }

    public void CheckPreviousNodes()
    {
        bool allUnlocked = true;

        foreach (var node in previousNodes)
        {
            if (!node.IsUnlocked)
            {
                allUnlocked = false;
                break;
            }
        }

        canBeUnlocked = allUnlocked && !isUnlocked;
        UpdateNodeVisuals();
    }

    public void Unlock()
    {
        IsUnlocked = true;
        canBeUnlocked = false;

        foreach (var node in nextNodes)
        {
            node.CheckPreviousNodes();
        }

        UpdateLinkColors(true);
        skillManager.UpdateSkullsText();
        UpdateNodeVisuals();
    }

    private void UpdateLinkColors(bool shouldHighlight)
    {
        Color targetColor = shouldHighlight ? new Color(1f, 0.5f, 0f) : Color.white;

        foreach (var link in links)
        {
            link.color = targetColor;
        }
    }

    private void UpdateNodeVisuals()
    {
        if (isUnlocked)
        {
            skillImage.color = Color.white;
            outlineImage.color = Color.green;
        }
        else if (canBeUnlocked)
        {
            skillImage.color = new Color(1, 1, 1, 0.7f);
            outlineImage.color = Color.yellow;
        }
        else
        {
            skillImage.color = new Color(1, 1, 1, 0.1f);
            outlineImage.color = Color.red;
        }
    }

    public void OnNodeClicked()
    {
        if (!canBeUnlocked || isUnlocked) return;

        int newSkullCount = skillManager.LoadSkullCount() - skillSO.skillCost;
        if (newSkullCount < 0) return;

        Unlock();
        SaveSystem.UpdatePlayerSkulls(newSkullCount);
        skillManager.UpdateSkullsText();
        SaveSystem.UpdateSkillTree(skillSO.id, true);
    }

    public void OnSkillHover()
    {
        skillManager.ShowDescriptionFrame(transform.position, skillSO.skillName, skillSO.skillDescription, skillSO.skillCost);
    }

    public void OnSkillHoverClear()
    {
        skillManager.HideDescriptionFrame();
    }

    public void ResetSkill()
    {
        isUnlocked = false;
        canBeUnlocked = false;
        UpdateLinkColors(false);
        CheckPreviousNodes(); // will call UpdateNodeVisuals as well
        SaveSystem.UpdateSkillTree(skillSO.id, false);
    }

  public void UnlockBasedOfSkillTreeData(int[] skillTreeNodesData)
{
    if (skillSO.id >= 0 && skillSO.id < skillTreeNodesData.Length && skillTreeNodesData[skillSO.id] == 1)
    {
        Unlock();
    }
}

}
