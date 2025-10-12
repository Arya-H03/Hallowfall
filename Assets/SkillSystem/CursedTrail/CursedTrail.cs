using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "CursedTrail", menuName = "Scriptable Objects/Skills/CursedTrail")]
public class CursedTrail : BaseSkillSO,IDamage,IChance,IAreaOfEffect,ILifeTime
{
    [SerializeField] GameObject cursedTrailPrefab;

    [SerializeField] private int damage;
    [SerializeField] private float chance;
    [SerializeField] private float effectSize;
    [SerializeField] private float lifeTime;
    public int Damage { get => damage; set => damage = value; }
    public float Chance { get => chance; set => chance = value; }
    public float AreaOfEffect { get => effectSize; set => effectSize = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    public override void ApplySkillLogic(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyHit += CursedSlashLogic;
        lvl = 1;
    }

    public override void LevelUpSkill(PlayerController playerController)
    {
        lvl++;
        Chance += 0.02f;
        LifeTime += 1;
        Damage += 2;
    }

    public override string GetSkillDescription()
    {
        return $"Your sword attacks have a <color=purple>{Chance * 100 + (2 * lvl)}%</color> chance to leave a cursed trail under hit enemies, lasting <color=yellow>{LifeTime + (1 * lvl)}s</color> and dealing damage over time.";
    }
    private void CursedSlashLogic(EnemyController enemy,int swordHitDmage)
    {
        if(!MyUtils.EvaluateChance(chance)) return;

        Vector3 cursedTrailSpawnPos = enemy.GetEnemyPos();
        Vector3 perfabTrailSize = cursedTrailPrefab.GetComponent<SpriteRenderer>().size;

        GameObject cursedTrailGO = Instantiate(cursedTrailPrefab, cursedTrailSpawnPos,Quaternion.Euler(60,0,0));
        cursedTrailGO.transform.localScale *= AreaOfEffect;
        cursedTrailGO.GetComponent<ShadowTrail>().Init(Damage);

        Destroy(cursedTrailGO, LifeTime);
    }

   
}
