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
    [SerializeField] GameObject necromancerPrefab;

    [SerializeField] private float mainWaveDelay = 5;
    [SerializeField] private int waveCounter = 0;
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
            waveCounter++;
            yield return new WaitForSeconds(mainWaveDelay);

            for(int i = 0; i < waveCounter; i++) SpawnEnemy(arsonistPrefab, GenerateRandomSpawnPosition(10));

            yield return new WaitForSeconds(1);
            for (int i = 0; i < waveCounter - 1 ; i++) SpawnEnemy(sinnerPrefab, GenerateRandomSpawnPosition(8));

            if (waveCounter >= 5)
            {
                mainWaveDelay += 5;
                for (int i = 0; i < (waveCounter / 5) + 1; i++) SpawnEnemy(necromancerPrefab, GenerateRandomSpawnPosition(8));
            }


        }
        
    }

    private void SpawnEnemy(GameObject prefab, Vector3 pos)
    {
        GameObject enemy = Instantiate(prefab, pos,Quaternion.identity);
        enemy.tag = "Enemy";
    }

    private Vector3 GenerateRandomSpawnPosition(float dist)
    {
        Vector3 randomOffset = Random.insideUnitCircle.normalized * dist;
        return GameManager.Instance.Player.transform.position + randomOffset;
    }
   
}
