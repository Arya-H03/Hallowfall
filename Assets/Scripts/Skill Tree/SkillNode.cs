using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [SerializeField] SkillNode [] previousNodes;
    [SerializeField] SkillNode [] nextNodes;

    [SerializeField] SkillSO skillSO;

    private SkillManager skillManager;
    

    [SerializeField] Image outlineImage;
    [SerializeField] Transform linkHolder;
    [SerializeField] Image skillImage;

    private List<Image> links = new List<Image>();

    private bool isUnlocked = false;
     private bool canBeUnlocked = false;

    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }

    private void Awake()
    {
        skillManager = GetComponentInParent<SkillManager>();
      
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
        if (previousNodes.Length == 0)
        {
            UpdateNode(true);
        } 
        else
        {
            bool temp = true;
            foreach (var node in previousNodes)
            {
                if (node.IsUnlocked != true)
                {
                    temp = false;
                    UpdateNode(temp);
                    return;
                }
            }
            if (temp) UpdateNode(true);
        }
           
    }

    private void UnlockNode()
    {
        IsUnlocked = true ;
        skillImage.color = new Color(1, 1, 1, 1f);
        outlineImage.color = Color.green;

        if (links.Count > 0)
        {
            foreach (var link in links)
            {
                link.color = new Color(1f, 0.5f, 0f);

            }
        }
        
        if(nextNodes.Length > 0)
        {
            foreach (var node in nextNodes)
            {
                node.CheckPreviousNodes();
            }
        }
       
    }

    private void UpdateNode(bool value)
    {
        if(value)
        {
            canBeUnlocked = true;
            skillImage.color = new Color(1, 1, 1, 0.7f);           
            outlineImage.color = Color.yellow;
        }
        else
        {
            canBeUnlocked = false;
            skillImage.color = new Color(1, 1, 1, 0.1f);           
            outlineImage.color = Color.red;
        }          
    }
    public void OnNodeClicked()
    {
        if(canBeUnlocked && !isUnlocked)
        {
            UnlockNode();
            skillSO.ApplySkill();
        }
    }

    public void OnSkillHover()
    {
        skillManager.ShowDescriptionFrame(this.transform.position,skillSO.name,skillSO.skillDescription,skillSO.skillCost);
    }
    public void OnSkillHoverClear()
    {
        skillManager.HideDescriptionFrame();
    }
}
