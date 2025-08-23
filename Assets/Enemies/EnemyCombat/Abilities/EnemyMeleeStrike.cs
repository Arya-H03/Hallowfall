using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMeleeStrike", menuName = "Scriptable Objects/Enemy Abilites/Enemy Melee Strike")]
public class EnemyMeleeStrike : BaseEnemyAbilitySO
{
    public EnemyAttackZone attackZonePrefab;
    private EnemyAttackZone attackZone;

    public int strikeDamage;
    public int parryDamage;

    public override void ExecuteAbility(EnemyController enemy)
    {
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, true);
        attackZone = Instantiate(attackZonePrefab,enemy.GetEnemyPos(),Quaternion.identity);
        SetupAttackZone(attackZone.gameObject,enemy);
        attackZone.Init(new EnemyMeleeStrikeData { owner = enemy, strikeDamage = this.strikeDamage, parryDamage = this.parryDamage });
        enemy.SignalHub.OnPlayRandomSFX?.Invoke(abilitySFX, 0.075f);
    }

    public override void ActionOnAnimFrame(EnemyController enemy)
    {
      
        if (attackZone != null) 
        {
            attackZone.TryHitTarget(enemy);            
            attackZone = null;
        }
        
    }

    public override void EndAbility(EnemyController enemy)
    {
        enemy.SignalHub.OnAnimBool?.Invoke(animCondition, false);
    }

    private void SetupAttackZone(GameObject attackZoneGO, EnemyController enemyController)
    {
        attackZoneGO.transform.parent = enemyController.transform;
        Vector3 dir = (enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;   
        attackZoneGO.transform.SetPositionAndRotation(enemyController.GetEnemyPos() + (dir / 2), Quaternion.Euler(0, 0, angle + 180));
    }


}

public struct EnemyMeleeStrikeData
{
    public EnemyController owner;
    public int strikeDamage;
    public int parryDamage;
}
