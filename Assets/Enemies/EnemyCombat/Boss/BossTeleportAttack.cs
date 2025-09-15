using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BossTeleportAttack", menuName = "Scriptable Objects/Boss Abilites/Boss Teleport Attack")]
public class BossTeleportAttack : BaseEnemyAbilitySO
{
    [SerializeField] EnemyAttackZone attackZonePrefab;
    [SerializeField] int attackDamage;
    [SerializeField] int parryDamage;
    private EnemyAttackZone attackZoneRef;
    
    public override void ExecuteAbility(EnemyController enemy)
    {
        enemy.CoroutineRunner.RunCoroutine(HandleTeleport(enemy));
    }
    public override void ActionOnAnimFrame(EnemyController enemy)
    {
        enemy.SignalHub.OnPlayRandomSFX?.Invoke(abilitySFX, 0.5f);
        if (attackZoneRef != null)
        {
            attackZoneRef.TryHitTarget(enemy);
            attackZoneRef = null;
        }

       
    }

    public override void EndAbility(EnemyController enemy)
    {
       
    }

    private IEnumerator HandleTeleport(EnemyController enemy)
    {
        float teleportDuration = (float)enemy.SignalHub.RequestAnimLength?.Invoke("Teleport");
        Vector3 teleportPos = enemy.PlayerGO.transform.position;
        enemy.SignalHub.OnAnimTrigger?.Invoke(animCondition);
        attackZoneRef = Instantiate(attackZonePrefab, teleportPos, Quaternion.identity);
        attackZoneRef.Init(new EnemyMeleeStrikeData { owner = enemy, strikeDamage = this.attackDamage, parryDamage = this.parryDamage });
        yield return new WaitForSeconds(teleportDuration);  
        enemy.transform.position = teleportPos;
        
        enemy.SignalHub.OnAnimTrigger?.Invoke("TeleportAttack");
       
    }


}
