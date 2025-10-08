using UnityEngine;


[CreateAssetMenu(fileName = "MassRetaliationSkill", menuName = "Scriptable Objects/Skills/MassRetaliationSkill")]
public class MassRetaliationSkill : BaseSkillSO, IDamage, IAreaOfEffect,IKnockbackForce
{
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] GameObject retaliationEffectPrefab;
    [SerializeField] int damage;
    [SerializeField] float effectSize;
    [SerializeField] float knockbackForce;
    [SerializeField] AudioClip [] effectSFX;
    public int Damage { get => damage; set => damage = value; }
    public float AreaOfEffect { get => effectSize; set => effectSize = value; }
    public float KnockbackForce { get => knockbackForce; set => knockbackForce = value; }

    public override string GetDescription()
    {
        return $"Successful parries trigger an explosion, dealing <color=red>{damage}</color> damage and knocking back nearby enemies.";

    }

    public override void Init(PlayerController controller)
    {
        controller.PlayerSignalHub.OnEnemyParried += (enemy, parryDamage) => MassRetaliationLogic(controller);
    }

    private void MassRetaliationLogic(PlayerController controller)
    {
        GameObject retaliationEffect = Instantiate(retaliationEffectPrefab, controller.GetPlayerPos(), Quaternion.identity);
        retaliationEffect.transform.localScale *= effectSize;
        Destroy(retaliationEffect, 0.5f);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(controller.GetPlayerPos(), (effectSize / 2) + 0.25f, retaliationEffect.transform.forward, 10, enemyLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.transform.parent.TryGetComponent<EnemyController>(out EnemyController enemyController))
            {
                enemyController.GetComponent<IHitable>().HandleHit(new HitInfo
                {
                    damage = Damage,
                    canBeImmune = false,
                    canFlashOnHit = true,
                    canPlayAnimOnHit = true,
                    canPlaySFXOnHit = true,
                    canPlayVFXOnHit = true,
                    knockbackInfo = new KnockbackInfo { canKnockback = true, forceSourcePosition = controller.GetPlayerPos(), knockbackForce = knockbackForce }
                });
                AudioManager.Instance.PlaySFX(effectSFX,controller.GetPlayerPos(),1f);
            }


        }
    }
}
