using TMPro;
using UnityEngine;

public class Objective : MonoBehaviour
{
    private TextMeshProUGUI textComp;
    private ObjectiveManager objectiveManager;
    private void Awake()
    {
        textComp = GetComponent<TextMeshProUGUI>(); 
    }

    public void Init(ObjectiveManager objectiveManager, Vector3 localPos,string text)
    {
        this.objectiveManager = objectiveManager;
        this.transform.SetParent(objectiveManager.transform, false);
        ChangePos(localPos);

        SetObjectiveText(text);
    }

    private void SetObjectiveText(string text)
    {
        textComp.text = text;
    }

    public void ChangePos(Vector3 pos)
    {
        this.transform.localPosition = pos;
    }
}
