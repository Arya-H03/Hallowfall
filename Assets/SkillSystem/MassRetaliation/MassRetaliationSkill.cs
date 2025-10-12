using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.InputSystem.XR;


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

    public override void ApplySkillLogic(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyParried += (enemy, parryDamage) => MassRetaliationLogic(playerController);
        lvl = 1;
    }
    public override void LevelUpSkill(PlayerController playerController)
    {
        lvl++;
        Damage += 10;
        KnockbackForce *= 1.2f;
    }
    public override string GetSkillDescription()
    {
        if (lvl == 0) return $"Successful parries trigger an explosion, dealing <color=red>{damage}</color> damage and knocking back nearby enemies.";
        else return $"Successful parries trigger an explosion, dealing <color=red>{damage +10}</color> damage and knocking back nearby enemies.";
         

    }
    private void MassRetaliationLogic(PlayerController controller)
    {
        GameObject retaliationEffect = Instantiate(retaliationEffectPrefab, controller.GetPlayerPos(), Quaternion.Euler(60,0,0));
        retaliationEffect.transform.localScale *= effectSize;
        Destroy(retaliationEffect, 0.33f);
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
