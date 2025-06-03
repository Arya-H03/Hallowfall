using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] GameObject skillDescriptionFrame;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;


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
}
