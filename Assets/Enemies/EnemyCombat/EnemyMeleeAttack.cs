using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyBaseAttack
{
    [SerializeField] LayerMask layerMask;
    protected EnemyAttackZone attackZone;

    [SerializeField] private int parryDamage = 100;

    private void Awake()
    {
        attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
    }
    //public override void CallAttackActionOnAnimFrame()
    //{
    //    PlayAttackSFX();
    //    if (attackZone.Target)
    //    {
    //        if (!attackZone.IsAttackParry)
    //        {
    //            attackZone.Target.GetComponent<PlayerController>().OnPlayerHit(AttackDamage);
    //        }
    //        else if (attackZone.ParryShield)
    //        {
    //            enemyController.EnemyPhysicsHandler.OnEnemyParried(attackZone.ParryShield, enemyController.PlayerController.GetPlayerPos(), parryDamage);
    //        }

    //    }

    //}
    //public override IEnumerator AttackCoroutine()
    //{

    //    isAvailable = false;
    //    SetupAttackZone();
    //    attackState.RemoveFromAvailableAttacks(this);
    //    enemyController.EnemyAnimationHandler.SetBoolForAnimation(AnimCondition, true);

    //    yield return new WaitForSeconds(attackCooldown);
    //    isAvailable = true;
    //    attackState.AddToAvailableAttacks(this);
    //}

    private void SetupAttackZone()
    {
        Vector3 dir = (enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle <= -90 || angle >= 90)
        {
            angle += 180;
        }
        attackZoneGO.transform.rotation = Quaternion.Euler(0, 0, angle);

        attackZoneGO.transform.position = enemyController.GetEnemyPos() + (dir / 2);

        attackZoneGO.SetActive(true);
    }
}
