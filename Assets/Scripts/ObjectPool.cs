using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject objPrefab;
    [SerializeField] int poolCount;
    [SerializeField] int poolIncreaseCount;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        GeneratePool(poolCount);
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

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }


}
