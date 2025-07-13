using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowClone : MonoBehaviour
{
    GameObject enemyTarget;
    private float damage = 50;


    public GameObject EnemyTarget { get => enemyTarget; set => enemyTarget = value; }

    public void DestroyClone()
    {
        Destroy(gameObject);
    }

    public void AttackEnemy()
    {
        enemyTarget.GetComponent<EnemyController>().OnEnemyHit(damage, enemyTarget.transform.position, HitSfxType.sword,0.5f);
    }


}
