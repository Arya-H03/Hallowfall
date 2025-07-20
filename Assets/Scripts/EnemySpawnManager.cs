using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    
    private static EnemySpawnManager instance;
    public static EnemySpawnManager Instance
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

    public GameObject ArsonistPrefab { get => arsonistPrefab; set => arsonistPrefab = value; }
    public GameObject SinnerPrefab { get => sinnerPrefab; set => sinnerPrefab = value; }
    public GameObject NecromancerPrefab { get => necromancerPrefab; set => necromancerPrefab = value; }

    [SerializeField] GameObject arsonistPrefab;
    [SerializeField] GameObject sinnerPrefab;
    [SerializeField] GameObject necromancerPrefab;

    [SerializeField] private float mainWaveDelay = 3;
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

            for(int i = 0; i < waveCounter; i++) SpawnEnemy(EnemyTypeEnum.Arsonist, GenerateRandomSpawnPosition(10));

            yield return new WaitForSeconds(1);
            for (int i = 0; i < waveCounter - 1 ; i++) SpawnEnemy(EnemyTypeEnum.Sinner, GenerateRandomSpawnPosition(8));

            if (waveCounter >= 5)
            {
                mainWaveDelay += 5;
                for (int i = 0; i < (waveCounter / 5) + 1; i++) SpawnEnemy(EnemyTypeEnum.Necromancer, GenerateRandomSpawnPosition(8));
            }


        }
        
    }

    public void SpawnEnemy(EnemyTypeEnum enemyType, Vector3 pos)
    {
        GameObject enemy = null;

        switch(enemyType)
        {
            case EnemyTypeEnum.Arsonist:
                enemy = ObjectPoolManager.Instance.ArsonistPool.GetFromPool();
            break;
            case EnemyTypeEnum.Revenant:
                enemy = ObjectPoolManager.Instance.RevenantPool.GetFromPool();
                break;
            case EnemyTypeEnum.Sinner:
                enemy = ObjectPoolManager.Instance.SinnerPool.GetFromPool();
                break;
            case EnemyTypeEnum.Necromancer:
                enemy = ObjectPoolManager.Instance.NecroPool.GetFromPool();
                break;

        }
        if (enemy != null)
        {
            enemy.transform.position = pos;
            enemy.transform.rotation = Quaternion.identity;
           
                                   
        }
    }

    private Vector3 GenerateRandomSpawnPosition(float dist)
    {
        Vector3 randomOffset = Random.insideUnitCircle.normalized * dist;
        return GameManager.Instance.Player.transform.position + randomOffset;
    }
   
}
