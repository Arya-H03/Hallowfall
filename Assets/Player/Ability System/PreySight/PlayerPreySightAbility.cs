using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPreySightAbility : MonoBehaviour,IAbility
{
    private CDetector cDetector;
    private CTargetMarker cTargetMarker;

    [SerializeField] int cycleDuration;

    private void Awake()
    {
        Init();
    }
    public void PassPlayerControllerRef(PlayerController playerController)
    {
      
    }
    public void Init()
    {
        cDetector = GetComponentInChildren<CDetector>();
        cTargetMarker = GetComponentInChildren<CTargetMarker>();

        MyUtils.ValidateFields(this, cDetector, "CDetector");
        MyUtils.ValidateFields(this, cTargetMarker, "CTargetMarker");
    }

    public void Perform()
    {
        StartCoroutine(PreySightCoroutine());
    }

    private IEnumerator PreySightCoroutine()
    {
        List<GameObject> markedEnemies = new List<GameObject>();

        while (true)
        {
            yield return new WaitForSeconds(cycleDuration);

           
            List<GameObject> detectedEnemies = cDetector.DetectNearbyTargets();


            List<GameObject> unmarkedEnemies = detectedEnemies.FindAll(e => !markedEnemies.Contains(e));

            if (unmarkedEnemies.Count > 0)
            {
                GameObject enemyToMark = unmarkedEnemies[Random.Range(0, unmarkedEnemies.Count)];

                cTargetMarker.MarkTarget(enemyToMark);
                markedEnemies.Add(enemyToMark);
            }
               
        }
    }
}
