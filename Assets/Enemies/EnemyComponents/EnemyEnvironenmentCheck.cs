
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//[RequireComponent (typeof(CDetector))]
public class EnemyEnvironenmentCheck : MonoBehaviour
{
    //private CDetector detector;
    //private EnemyController enemyController;

    //[SerializeField] private LayerMask enemyLayerMask;
    //[SerializeField] private string enemyTag = "Enemy";
    //[SerializeField] private float seperationCheckRadius = 1f;
    //[SerializeField] private float seperationStrength = 1f;

    //[SerializeField] List<GameObject> nearbyEnemies = new();
    //[SerializeField] float disttp;
    //[SerializeField] bool isn;
    //private void Awake()
    //{
    //    detector = GetComponent<CDetector>();
    //    enemyController = GetComponentInParent<EnemyController>();
      
    //}

    //public Vector2 CalculateSeparationForce()
    //{
    //    Vector2 seperationForce = Vector2.zero;
    //    int neighborCount = 0;

    //    List <GameObject> nearbyEnemies = detector.DetectNearbyTargets(enemyTag, enemyLayerMask, seperationCheckRadius);
    //    foreach (GameObject enemy in nearbyEnemies)
    //    {
    //        if (enemy == this.gameObject) continue;

    //        Vector2 enemyForce = (Vector2)(enemyController.transform.position - enemy.transform.position);
    //        if(enemyForce.magnitude >0)
    //        {
    //            seperationForce += enemyForce / enemyForce.magnitude;
    //            //Debug.DrawLine(enemyController.GetEnemyPos(), enemyController.GetEnemyPos() + (Vector3)seperationForce,Color.red);
    //            neighborCount++;

    //        }
    //    }

    //    if (neighborCount > 0) 
    //    {
    //        //seperationForce /= neighborCount;
    //        seperationForce = seperationForce.normalized;
    //        //Debug.DrawLine(enemyController.GetEnemyPos(), enemyController.GetEnemyPos() + (Vector3)seperationForce, Color.green);
    //    }

    //    return seperationForce;
    //}

 
}
