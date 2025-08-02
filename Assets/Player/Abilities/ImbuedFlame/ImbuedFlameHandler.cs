using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ImbuedFlameHandler : ActiveAbilityHandler
{
    [SerializeField] GameObject flamePrefab;

    PlayerSwordAttackState swordAttackState;
    PlayerController playerController;

    private void Start()
    {
        swordAttackState = GameManager.Instance.Player.GetComponentInChildren<PlayerSwordAttackState>();
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        //swordAttackState.OnFirstSwordSwingEvent += SpawnFirstSwingFlame;
        //swordAttackState.OnSecondSwordSwingEvent += SpawnSecondSwingFlame;
        //swordAttackState.OnThirdSwordSwingEvent += SpawnThirdSwingFlame;
    }

    private void SpawnFirstSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, playerController.GetPlayerPos(), Quaternion.Euler(0, 0, 0));


        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseToCursor();

    }

    private void SpawnSecondSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, playerController.GetPlayerPos(), Quaternion.Euler(0,0,0));
        // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);
        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseToCursor();



    }

    private void SpawnThirdSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, playerController.GetPlayerPos(), Quaternion.Euler(0, 0, 0));
        // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);

        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseToCursor();

    }
}
