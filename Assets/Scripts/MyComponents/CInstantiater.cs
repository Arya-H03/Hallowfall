using UnityEngine;

public class CInstantiater : MonoBehaviour
{
    public GameObject InstantiateGO(GameObject go, Vector3 pos, Quaternion rot)
    {
        GameObject returnObj = Instantiate(go, pos, rot);
        return returnObj;
    }

    public GameObject InstantiateGOUnderParent(GameObject go, Vector3 pos, Quaternion rot, Transform parent )
    {
        GameObject returnObj = Instantiate(go, pos, rot, parent);
        returnObj.transform.localPosition = Vector3.zero;
        return returnObj;
    }
}
