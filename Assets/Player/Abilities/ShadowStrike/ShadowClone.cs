using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowClone : MonoBehaviour
{
    GameObject enemyTarget;
    private int damage ;
    public GameObject EnemyTarget { get => enemyTarget; set => enemyTarget = value; }
    public int Damage { get => damage; set => damage = value; }

    public void DestroyClone()
    {
        Destroy(gameObject);
    }

    public void AttackEnemy()
    {
        enemyTarget.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = damage, HitSfx = HitSfxType.sword, AttackerPosition = this.transform.position, KnockbackForce = 1 }); ;
    }


}
