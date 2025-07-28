using UnityEngine;

public class CSpawner : MonoBehaviour
{
   
    public GameObject Spawn(GameObject gameObjectPrefab, Vector3 position,Quaternion rotation,  Transform parent = null)
    {
        GameObject spawnedGameObject = Instantiate(gameObjectPrefab,position, rotation);
        if(parent != null) spawnedGameObject.transform.parent = parent;

        return spawnedGameObject;
    }

   
}
