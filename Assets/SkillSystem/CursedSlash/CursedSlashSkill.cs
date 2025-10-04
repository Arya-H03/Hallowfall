using UnityEngine;

[CreateAssetMenu(fileName = "CursedSlashSkill", menuName = "Scriptable Objects/Skills/CursedSlashSkill")]
public class CursedSlashSkill : BaseSkillSO,IDamage,IChance,IAreaOfEffect,ILifeTime
{
    [SerializeField] GameObject cursedTrailPrefab;

    private EntityController ownerEntity;

    [SerializeField] private int damage;
    [SerializeField] private float chance;
    [SerializeField] private float effectSize;
    [SerializeField] private float lifeTime;
    public int Damage { get => damage; set => damage = value; }
    public float Chance { get => chance; set => chance = value; }
    public float AreaOfEffect { get => effectSize; set => effectSize = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    public override void Init(EntityController controller)
    {
        if (controller == null) return;
        ownerEntity = controller;
        if(ownerEntity is PlayerController playerController)
        {
            playerController.PlayerSignalHub.OnEnemyHit += CursedSlashLogic;
        }
    }
    private void CursedSlashLogic(EnemyController enemy,int swordHitDmage)
    {
        if(!MyUtils.EvaluateChance(chance)) return;

        Vector3 cursedTrailSpawnPos = enemy.GetEnemyPos();
        Vector3 perfabTrailSize = cursedTrailPrefab.GetComponent<SpriteRenderer>().size;

        GameObject cursedTrailGO = Instantiate(cursedTrailPrefab, cursedTrailSpawnPos,Quaternion.Euler(60,0,0));
        cursedTrailGO.transform.localScale *= AreaOfEffect;
        cursedTrailGO.GetComponent<ShadowTrail>().Init(ownerEntity, Damage, ownerEntity.EntityType);

        Destroy(cursedTrailGO, LifeTime);
    }
}
