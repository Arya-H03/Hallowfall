using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    private EnemySpawnManager instance;
    public EnemySpawnManager Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject go = new GameObject("EnemySpawnManager");
                instance = go.AddComponent<EnemySpawnManager>();
            }
            return instance;
        }
    }

    [SerializeField] GameObject arsonistPrefab;
    [SerializeField] GameObject sinnerPrefab;

    private float mainWaveDelay = 5;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(MainEnemySpawnCoroutine());
    }

    private IEnumerator MainEnemySpawnCoroutine()
    {
        while (true) 
        {
            yield return new WaitForSeconds(mainWaveDelay);
            SpawnEnemy(arsonistPrefab, GenerateRandomSpawnPosition(10));
            yield return new WaitForSeconds(1);
            SpawnEnemy(sinnerPrefab, GenerateRandomSpawnPosition(8));
        }
        
    }

    private void SpawnEnemy(GameObject prefab, Vector3 pos)
    {
        GameObject enemy = Instantiate(prefab, pos,Quaternion.identity);
    }

    private Vector3 GenerateRandomSpawnPosition(float dist)
    {
        Vector3 randomOffset = Random.insideUnitCircle.normalized * dist;
        return GameManager.Instance.Player.transform.position + randomOffset;
    }
   
}
