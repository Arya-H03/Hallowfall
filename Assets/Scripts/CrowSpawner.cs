using UnityEngine;

public class CrowSpawner : MonoBehaviour
{
    [SerializeField] GameObject crowPrefab;
    [SerializeField] int minSpawnCount = 1;
    [SerializeField] int maxSpawnCount = 3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SpawnCrow();
        }
    }

    private void SpawnCrow()
    {
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
        for(int i = 0; i< spawnCount; i++)
        {
            GameObject crow = Instantiate(crowPrefab, transform.position + MyUtils.GetRandomOffset(1), Quaternion.identity);
        }
        
    }
}
