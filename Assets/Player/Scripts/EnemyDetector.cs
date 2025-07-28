using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private PlayerEnvironmentChecker environmentCheck;
    private CDetector cDetector;

    [SerializeField] private float checkRadius = 1.0f;
    private List<GameObject> enemies = new();
  
    void Start()
    {
        environmentCheck = GetComponentInParent<PlayerEnvironmentChecker>();
        cDetector = environmentCheck.Detector;
    }
    private void FixedUpdate()
    {
        CheckForEnemies();
    }
    private void CheckForEnemies()
    {
        enemies.Clear();
        enemies = cDetector.DetectNearbyTargets("Enemy", MyUtils.GetMousePos(),environmentCheck.EnemyLayerMask, checkRadius,true);
    }

    
}
