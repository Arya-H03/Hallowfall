using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemDropHandler : MonoBehaviour,IInitializeable<EnemyController>
{
    private EnemySignalHub signalHub;
    private EnemyController enemyController;
   

    [SerializeField] float skullDropChance = 0.50f;
    [SerializeField] int skullDropCount = 1;

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        signalHub  = enemyController.SignalHub;

        signalHub.OnItemDrop += HandleItemDrop;
    }

    //private void OnDisable()
    //{
    //    if (signalHub == null) return;
    //    signalHub.OnItemDrop -= HandleItemDrop;
    //}

    private void HandleItemDrop()
    {      
        DropItem(ObjectPoolManager.Instance.SkullPool, skullDropChance, skullDropCount, enemyController.GetEnemyPos());
    }

    private void DropItem(ObjectPool pool, float dropChance, int count, Vector3 pos)
    {
        if(!MyUtils.EvaluateChance(dropChance)) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            var go = pool.GetFromPool();
            go.transform.position = offset + pos;
            go.GetComponent<BaseItem>().OnItemDrop();
        }

    }




}
