using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazingArcHandler : ActiveAbilityHandler
{
    [SerializeField] SpawnerAbility blazingArcSO;
    private int currentSpawnCount;
    private float currentSpawnDelay;

    private GameObject player; 
    private PlayerController playerController; 

    private void Start()
    {
        player = GameManager.Instance.Player;
        playerController = player.GetComponent<PlayerController>();

        currentSpawnCount = blazingArcSO.spawnCount;
        currentSpawnDelay = blazingArcSO.spawnDelay;

        StartCoroutine(SpawnBlazingArcCoroutine());
    }

    private IEnumerator SpawnBlazingArcCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnDelay);

            for (int i = 0; i < currentSpawnCount; i++)
            {
                Vector3 spawnPos = player.transform.position + new Vector3(playerController.PlayerSignalHub.FacingDirctionBinding.Getter().x, player.GetComponent<SpriteRenderer>().bounds.size.y/2,0); 
                GameObject effect = Instantiate(blazingArcSO.projectile, spawnPos, Quaternion.identity);
                effect.GetComponent<PlayerProjectiles>().SetProjectileCourseForward(player);
                AudioManager.Instance.PlaySFX(sfx, playerController.transform.position, 1);
            }

        }

    }
}
