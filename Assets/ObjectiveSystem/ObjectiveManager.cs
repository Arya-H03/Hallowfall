using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private static ObjectiveManager instance;
    public static ObjectiveManager Instance
    {
        get { return instance;}
    }

    [SerializeField] private Objective objectivePrefab;
    [SerializeField] private GameObject headerFrame;

    private List<Objective> objectiveList = new();
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }    
    public Objective CreateObjectiveBox(string text)
    {
        Objective objective = Instantiate(objectivePrefab);
        objectiveList.Add(objective);
        objective.Init(this, new Vector3(0, 65 - (objectiveList.IndexOf(objective) * 35), 0), text);

        return objective;
    }

    public void RemoveObjective(Objective objective)
    {
        int objectiveIndex = objectiveList.IndexOf(objective);
        for (int i = objectiveIndex + 1; i < objectiveList.Count; i++)
        {
            objectiveList[i].ChangePos(new Vector3(0, 65 - ((i - 1) * 35), 0));
        }
        objectiveList.Remove(objective);
        Destroy(objective.gameObject); 
    }
}
