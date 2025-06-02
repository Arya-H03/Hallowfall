using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    private Image image;

    [SerializeField] SkillNode [] previousNodes;
    [SerializeField] SkillNode [] nextNodes;
    private List<Image> links = new List<Image>();

    private bool isUnlocked = false;
     private bool canBeUnlocked = false;

    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }

    private void Awake()
    {
        image = GetComponent<Image>();

        foreach(Transform child in transform)
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
        image.color = Color.green;

        if (links.Count > 0)
        {
            foreach (var link in links)
            {
                link.color = Color.cyan;

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
            image.color = Color.yellow;
        }
        else
        {
            canBeUnlocked = false;  
            image.color = Color.red;
        }          
    }
    public void OnNodeClicked()
    {
        if(canBeUnlocked && !isUnlocked)
        {
            UnlockNode();
        }
    }
}
