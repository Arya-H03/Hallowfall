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
    [SerializeField] GameObject undeadPrefab;
    [SerializeField] GameObject death;

    [SerializeField] private float mainWaveDelay = 3;
    [SerializeField] private int waveCounter = 0;
    [SerializeField] private float bossSpawnDelay = 2;

    [SerializeField] AudioClip[] waveIncomingSFX;
    

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
        StartCoroutine(SpawnBossCoroutine());
    }

    private IEnumerator MainEnemySpawnCoroutine()
    {
        while (true) 
        {
            waveCounter++;
            yield return new WaitForSeconds(mainWaveDelay);
            AudioManager.Instance.PlaySFX(waveIncomingSFX,GameManager.Instance.Player.transform.position, 1);
            for(int i = 0; i < waveCounter; i++) SpawnEnemy(EnemyTypeEnum.Undead, GenerateRandomSpawnPosition());

            yield return new WaitForSeconds(1);
            for (int i = 0; i < waveCounter - 3 ; i++) SpawnEnemy(EnemyTypeEnum.Spectrum, GenerateRandomSpawnPosition());

            if (waveCounter >= 3)
            {
                mainWaveDelay += 1;
                for (int i = 0; i < (waveCounter / 3) + 1; i++) SpawnEnemy(EnemyTypeEnum.Undead, GenerateRandomSpawnPosition());
                for (int i = 0; i < (waveCounter / 3) + 1; i++) SpawnEnemy(EnemyTypeEnum.Sinner, GenerateRandomSpawnPosition());
            }



            mainWaveDelay += 1;

        }
        
    }

    private IEnumerator SpawnBossCoroutine()
    {
        UIManager.Instance.BossTimer.StartBossTimer(bossSpawnDelay);
        yield return new WaitForSeconds(bossSpawnDelay);
        GameObject bossGO = Instantiate(death, GenerateRandomSpawnPosition(), Quaternion.identity);
        EnemyController enemyController = bossGO.GetComponent<EnemyController>();
        BossHealthBarHandler.Instance.SetupHealthbar(enemyController);
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
            case EnemyTypeEnum.Undead:
                enemy = ObjectPoolManager.Instance.UndeadPool.GetFromPool();
                break;
            case EnemyTypeEnum.Spectrum:
                enemy = ObjectPoolManager.Instance.SpectrumPool.GetFromPool();
                break;

        }
        if (enemy != null)
        {
            enemy.transform.position = pos;
            enemy.transform.rotation = Quaternion.identity;
           
                                   
        }
    }

    private Vector3 GenerateRandomSpawnPosition()
    {
        Vector3? result = null;
        while(result == null)
        {
            Vector3 temp  = PlayerCameraHandler.Instance.GetRandomOffScreenPos(1);

            if(ZoneManager.Instance.FindCurrentCellFromWorldPos((Vector3)temp).IsWalkable)
            {
                result = temp;
                break;   
            }
        }

        return (Vector3)result;

       

        //Vector3 randomOffset = Random.insideUnitCircle.normalized * dist;
        //return GameManager.Instance.PlayerGO.transform.position + randomOffset;
    }
   
}
