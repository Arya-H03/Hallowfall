using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CDetector : MonoBehaviour
{
    public List<GameObject> DetectNearbyGameObjectTargets(string tTag,Vector3 pos, LayerMask tLayerMask, float searchRadius, bool debug = false)
    {
        List<GameObject> targetList = new List<GameObject>();

        RaycastHit2D[] targets = Physics2D.CircleCastAll(pos, searchRadius, Vector2.zero, tLayerMask);
        foreach (RaycastHit2D target in targets)
        {
            if (target.collider.CompareTag(tTag))
            {
                targetList.Add(target.collider.gameObject);
            }
        }
        if (debug)
        {
            DrawDebugCircle(pos, searchRadius, Color.red);
  
        }
        return targetList;
    }

    public List<T> DetectNearbyGenericTargets<T>(string tTag, Vector3 pos, LayerMask tLayerMask, float searchRadius, bool debug = false)
    {
        List<T> targetList = new List<T>();

        RaycastHit2D[] targets = Physics2D.CircleCastAll(pos, searchRadius, Vector2.zero, tLayerMask);
        foreach (RaycastHit2D target in targets)
        {
            if (target.collider.CompareTag(tTag) && target.collider.gameObject.TryGetComponent<T>(out T comp))
            {
                targetList.Add(comp);
            }
        }
        if (debug)
        {
            DrawDebugCircle(pos, searchRadius, Color.red);

        }
        return targetList;
    }

    public List<T> DetectNearbyGenericTargetsOnParent<T>(string tTag, Vector3 pos, LayerMask tLayerMask, float searchRadius, bool debug = false)
    {
        List<T> targetList = new List<T>();

        RaycastHit2D[] targets = Physics2D.CircleCastAll(pos, searchRadius, Vector2.zero, tLayerMask);
        foreach (RaycastHit2D target in targets)
        {
            if (target.collider.CompareTag(tTag) && target.collider.gameObject.transform.parent.TryGetComponent<T>(out T comp))
            {
                targetList.Add(comp);
            }
        }
        if (debug)
        {
            DrawDebugCircle(pos, searchRadius, Color.red);

        }
        return targetList;
    }


    public List<GameObject> DetectNearbyTargetsExcludingSelf(GameObject caller,string tTag, LayerMask tLayerMask, float searchRadius, bool debug = false)
    {
        List<GameObject> targetList = new List<GameObject>();

        RaycastHit2D[] targets = Physics2D.CircleCastAll(this.transform.position, searchRadius, Vector2.zero, tLayerMask);
        foreach (RaycastHit2D target in targets)
        {
            if (target.collider.CompareTag(tTag) && caller != target.collider.gameObject)
            {
                targetList.Add(target.collider.gameObject);
            }
        }
        if (debug)
        {
            DrawDebugCircle(this.transform.position, searchRadius, Color.red);
            Debug.Log(targetList.Count);
        }
        return targetList;
    }

    private void DrawDebugCircle(Vector3 center, float radius, Color color, int segments = 32)
    {
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = Mathf.Deg2Rad * (i * angleStep);
            float angle2 = Mathf.Deg2Rad * ((i + 1) * angleStep);

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Debug.DrawLine(point1, point2, color, 0.01f); // Duration = 0.01 to appear for 1 frame
        }
    }

}
