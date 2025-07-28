using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CTicker))]
public class EnemyDetector : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerEnvironmentChecker environmentCheck;
    private CDetector cDetector;
    private CTicker cTicker;
    private int propertyId = Shader.PropertyToID("_HasOut");

    [SerializeField] private float checkRadius = 1.0f;

    private HashSet<EnemyController> availableEnemyTargets = new();
    private HashSet<EnemyController> newEnemyTargets = new();

    public HashSet<EnemyController> AvailableEnemyTargets { get => availableEnemyTargets; }

    private void Awake()
    {
        cTicker = GetComponent<CTicker>();
    }
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        environmentCheck = GetComponentInParent<PlayerEnvironmentChecker>();
        cDetector = environmentCheck.Detector;


        cTicker.OnTickEvent += CheckForEnemies;
        cTicker.CanTick = true;
    }

   
    private void CheckForEnemies()
    {
        newEnemyTargets = cDetector.DetectNearbyGenericTargetsOnParent<EnemyController>("EnemyCollider", FindDetectorPos(), environmentCheck.EnemyLayerMask, checkRadius, true).ToHashSet();

        foreach (EnemyController oldEnemyTarget in availableEnemyTargets)
        {
            if(!newEnemyTargets.Contains(oldEnemyTarget)) oldEnemyTarget.Material.SetFloat(propertyId, 0);

        }
       
        foreach (EnemyController newEnemyTarget in newEnemyTargets)
        {
            if (!availableEnemyTargets.Contains(newEnemyTarget)) newEnemyTarget.Material.SetFloat(propertyId, 1);
        }
        availableEnemyTargets = newEnemyTargets;
    }


   private Vector3 FindDetectorPos()
   {
        return playerController.GetPlayerPos() + ((MyUtils.GetMousePos() - playerController.GetPlayerPos()).normalized * 1f) ;
   }

}
