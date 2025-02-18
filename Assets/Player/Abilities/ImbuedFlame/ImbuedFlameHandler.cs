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
        audioSource = GetComponent<AudioSource>();
        swordAttackState.OnFirstSwordSwingEvent += SpawnFirstSwingFlame;
        swordAttackState.OnSecondSwordSwingEvent += SpawnSecondSwingFlame;
        swordAttackState.OnThirdSwordSwingEvent += SpawnThirdSwingFlame;
    }

    private void SpawnFirstSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.FirstSwingCenter.position, Quaternion.Euler(0, 0, 0));
        //AudioManager.Instance.PlayRandomSFX(audioSource, sfx);

        Vector3 scale = GameManager.Instance.Player.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 3;
        Destroy(obj,0.5f);

    }

    private void SpawnSecondSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.SecondSwingCenter.position, Quaternion.Euler(0,0,0));
       // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);


        Vector3 scale = GameManager.Instance.Player.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 3;
        Destroy(obj, 0.5f);

    }

    private void SpawnThirdSwingFlame()
    {
        GameObject obj = Instantiate(flamePrefab, swordAttackState.SecondSwingCenter.position, Quaternion.Euler(0, 0, 0));
       // AudioManager.Instance.PlayRandomSFX(audioSource, sfx);


        Vector3 scale = GameManager.Instance.Player.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 3;
        Destroy(obj, 0.5f);

    }
}
