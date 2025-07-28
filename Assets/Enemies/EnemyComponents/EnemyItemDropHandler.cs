using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemDropHandler : MonoBehaviour
{
   
    [SerializeField] float essenceDropChance = 0.75f;
    [SerializeField] float skullDropChance = 0.50f;

    [SerializeField] int essenceDropCount = 1;
    [SerializeField] int skullDropCount = 1;

    public void HandleItemDrop(Vector3 pos)
    {      
        DropItem(ObjectPoolManager.Instance.EssencePool, essenceDropChance, essenceDropCount, pos);
        DropItem(ObjectPoolManager.Instance.SkullPool, skullDropChance, skullDropCount, pos);
    }

    private void DropItem(ObjectPool pool, float dropChance, int count, Vector3 pos)
    {
        int rand = Random.Range(1,101);
        if (rand < dropChance * 100)
        {
            for (int i = 0; i < count; i++)
            {
                //Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                ////var go = Instantiate(item, pos + offset, Quaternion.identity);
                //var go = pool.GetFromPool();
                //go.transform.position = offset + pos;
                //go.GetComponent<BaseItem>().OnItemDrop();
            }
        }

    }




}
