using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objPrefab;
    [SerializeField] int poolCount;
    [SerializeField] int poolIncreaseCount;
    [SerializeField] bool canGeneratePoolOnStart = true;    

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        if(canGeneratePoolOnStart)
        {
            GeneratePool(poolCount);
        }
        
    }
    private void GeneratePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(objPrefab,Vector3.zero,Quaternion.identity);
            obj.SetActive(false);
            obj.transform.parent = transform;
            pool.Enqueue(obj);
        }
    }

    public void GeneratePool()
    {
        GeneratePool(poolCount);
    }

    public GameObject GetFromPool()
    {
        if(pool.Count > 0 )
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GeneratePool(poolIncreaseCount);
            return GetFromPool();
        }
    }

    public GameObject GetFromPool(Vector3 pos, Quaternion rotation, Transform parent = null)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.transform.position = pos;
            obj.transform.rotation = rotation;
            if(parent) obj.transform.parent = parent;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GeneratePool(poolIncreaseCount);
            return GetFromPool(pos, rotation,parent);
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ReturnToPool(GameObject obj, float delay)
    {
        StartCoroutine(ReturnToPoolWithDelay(obj, delay));
    }

    private IEnumerator ReturnToPoolWithDelay(GameObject obj,float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obj);
    }

}
