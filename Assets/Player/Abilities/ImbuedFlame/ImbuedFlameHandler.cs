using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ImbuedFlameHandler : ActiveAbilityHandler
{
    [SerializeField] GameObject flamePrefab;

    PlayerSwordAttackState swordAttackState;

    private void Start()
    {
        swordAttackState = GameManager.Instance.Player.GetComponentInChildren<PlayerSwordAttackState>();
        swordAttackState.OnFirstSwordSwingEvent += SpawnFirstSwingFlame;
        swordAttackState.OnSecondSwordSwingEvent += SpawnSecondSwingFlame;
        swordAttackState.OnThirdSwordSwingEvent += SpawnThirdSwingFlame;
    }

    private void SpawnFirstSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.FirstSwingCenter.position, Quaternion.Euler(0, 0, 0));
      

        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseForward(GameManager.Instance.Player);

    }

    private void SpawnSecondSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.SecondSwingCenter.position, Quaternion.Euler(0,0,0));
        // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);
        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseForward(GameManager.Instance.Player);

        

    }

    private void SpawnThirdSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.SecondSwingCenter.position, Quaternion.Euler(0, 0, 0));
        // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);

        obj.GetComponent<PlayerProjectiles>().SetProjectileCourseForward(GameManager.Instance.Player);

    }
}
