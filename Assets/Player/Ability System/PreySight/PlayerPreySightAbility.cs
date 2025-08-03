using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPreySightAbility : MonoBehaviour,IAbility
{
    private CDetector cDetector;
    private CTargetMarker cTargetMarker;
    private PlayerController playerController;
    private LayerMask layerMask;
    private string enemyTag;

    [SerializeField] private float detectionRadius = 7.5f;
    [SerializeField] int cycleDuration;

    private void Awake()
    {
        Init();
    }
    public void PassPlayerControllerRef(PlayerController playerController)
    {
        this.playerController = playerController;
        layerMask = playerController.PlayerConfig.enemyMask;
        enemyTag = playerController.EnemyTag;
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

           
            List<GameObject> detectedEnemies = cDetector.DetectNearbyGameObjectTargets(enemyTag, playerController.GetPlayerPos(),layerMask, detectionRadius);


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
