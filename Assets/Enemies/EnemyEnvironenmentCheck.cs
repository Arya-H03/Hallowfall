
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CDetector))]
public class EnemyEnvironenmentCheck : MonoBehaviour
{
    private CDetector detector;
    private EnemyController enemyController;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float seperationCheckRadius = 2f;
    [SerializeField] private float seperationStrength = 1f;

    private void Awake()
    {
        detector = GetComponent<CDetector>();
        enemyController = GetComponentInParent<EnemyController>();
      
    }

    public Vector2 CalculateSeparationForce()
    {
        Vector2 seperationForce = Vector2.zero;
        int neighborCount = 0;

        List <GameObject> nearbyEnemies = detector.DetectNearbyTargets(enemyTag, enemyLayerMask, seperationCheckRadius);
        foreach (GameObject enemy in nearbyEnemies)
        {
            if (enemy == this.gameObject) continue;

            Vector2 enemyForce = (Vector2)(enemyController.GetEnemyCenter() - enemy.transform.position);
            if(enemyForce.magnitude >0)
            {
                seperationForce += enemyForce.normalized / enemyForce.magnitude;
                //Debug.DrawLine(enemyController.GetEnemyCenter(), enemyController.GetEnemyCenter() + (Vector3)seperationForce,Color.red);
                neighborCount++;

            }
        }

        if (neighborCount > 0) 
        {
            seperationForce /= neighborCount;
            seperationForce = seperationForce.normalized * seperationStrength;
            //Debug.DrawLine(enemyController.GetEnemyCenter(), enemyController.GetEnemyCenter() + (Vector3)seperationForce, Color.green);
        }

        return seperationForce;
    }
}
