using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyBaseAttack
{
    [SerializeField] private Vector2 boxCastSize = new Vector2(1.75f, 0.5f);
    [SerializeField] Transform boxCastCenter;
    private float distance = 0;
    [SerializeField] LayerMask layerMask;
    protected EnemyAttackZone attackZone;

    [SerializeField] private int parryDamage = 100;

    protected override void Awake()
    {
        base.Awake();
        attackZone = attackZoneGO.GetComponent<EnemyAttackZone>();
    }
    public override void CallAttackActionOnAnimFrame()
    {
        PlayAttackSFX();
        if (attackZone.Target)
        {
            if (!attackZone.IsAttackParry)
            {
                attackZone.Target.GetComponent<PlayerController>().OnPlayerHit(AttackDamage);
            }
            else if(attackZone.ParryShield)
            {
                enemyController.collisionManager.OnEnemyParried(attackZone.ParryShield, enemyController.transform.position, parryDamage);
            }
           
        }
        
       

    }
    public override IEnumerator AttackCoroutine()
    {
        
        isAvailable = false;
        SetupAttackZone();
        enemyController.AttackState.RemoveFromAvailableAttacks(this);
        enemyController.EnemyAnimationManager.SetBoolForAnimation(AnimCondition, true);
        
        yield return new WaitForSeconds(attackCooldown);
        isAvailable = true;
        enemyController.AttackState.AddToAvailableAttacks(this);
    }

    private void SetupAttackZone()
    {
        Vector3 dir = (enemyController.PlayerController.GetPlayerCenter() - enemyController.GetEnemyCenter()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle <= -90 || angle >= 90)
        {
            angle += 180;
        }
        attackZoneGO.transform.rotation = Quaternion.Euler(0, 0, angle);

        attackZoneGO.transform.position = enemyController.GetEnemyCenter() + (dir/2);

        attackZoneGO.SetActive(true);
    }
}
