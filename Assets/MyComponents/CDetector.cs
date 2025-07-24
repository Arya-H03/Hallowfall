using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CDetector : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float searchRadius;

    public List<GameObject> DetectNearbyTargets()
    {
        List<GameObject> targetList = new List<GameObject>();

        RaycastHit2D [] targets = Physics2D.CircleCastAll(this.transform.position,searchRadius,Vector2.zero,targetLayerMask);
        foreach (RaycastHit2D target in targets )
        {
            if(target.collider.CompareTag(targetTag))
            {
                targetList.Add(target.collider.gameObject);
            }
        }
        return targetList;
    }

    public List<GameObject> DetectNearbyTargets(string tTag, LayerMask tLayerMask, float searchRadius)
    {
        List<GameObject> targetList = new List<GameObject>();

        RaycastHit2D[] targets = Physics2D.CircleCastAll(this.transform.position, searchRadius, Vector2.zero, tLayerMask);
        foreach (RaycastHit2D target in targets)
        {
            if (target.collider.CompareTag(tTag))
            {
                targetList.Add(target.collider.gameObject);
            }
        }
        return targetList;
    }
}
