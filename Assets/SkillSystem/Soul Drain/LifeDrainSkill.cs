using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LifeDrainSkill", menuName = "Scriptable Objects/Skills/LifeDrainSkill")]
public class LifeDrain : BaseSkillSO, IAreaOfEffect,ICooldown,ILifeTime,IDamage
{
    [SerializeField] GameObject soulDrainEffectPrefab;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] private int damage;
    [SerializeField] private float cooldown;
    [SerializeField] private float effectSize;
    [SerializeField] private float lifeTime;

    public float AreaOfEffect { get => effectSize; set => effectSize = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public int Damage { get => damage; set => damage = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    public override void Init(PlayerController controller)
    {
        if (controller == null) return;
        controller.CoroutineRunner.StartCoroutine(SpawnSoulDrainCircleCoroutine(controller));
    }

    public override string GetDescription()
    {
        return $"A life draining circle appears around you every <color=yellow>{Cooldown}s</color>, lasting <color=yellow>{lifeTime}s</color>. It damages nearby enemies and heals you for a portion of the damage dealt.";
    }

    private IEnumerator SpawnSoulDrainCircleCoroutine(PlayerController playerController)
    {
        Coroutine soulDrainLogicCoroutine;

        yield return new WaitForSeconds(2f);
        while (true)
        {
            
            GameObject soulDrainEffect = Instantiate(soulDrainEffectPrefab, playerController.GetPlayerPos(), Quaternion.identity);
            soulDrainEffect.transform.parent = playerController.transform;
            soulDrainEffect.transform.localPosition = Vector3.zero;
            soulDrainEffect.transform.localScale = Vector3.one * effectSize;

            soulDrainLogicCoroutine = playerController.CoroutineRunner.StartCoroutine(SoulDrainLogic(playerController,soulDrainEffect));
            yield return new WaitForSeconds(lifeTime);
            playerController.CoroutineRunner.EndCoroutine(soulDrainLogicCoroutine);
            Destroy(soulDrainEffect);
            yield return new WaitForSeconds(cooldown);
        }
    }

    private IEnumerator SoulDrainLogic(PlayerController playerController,GameObject soulDrainEffect)
    {
        while (true)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(soulDrainEffect.transform.position, effectSize / 2, soulDrainEffect.transform.forward,10,enemyLayerMask); 
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider != null && hit.collider.transform.parent.TryGetComponent<EnemyController>(out EnemyController enemyController ))
                {
                    enemyController.GetComponent<IHitable>().HandleHit(new HitInfo
                    {
                        damage = Damage,
                        canBeImmune = false,
                        canFlashOnHit = true,
                        canPlayAnimOnHit = false,
                        canPlaySFXOnHit = false,
                        canPlayVFXOnHit = false
                    });
                    playerController.PlayerSignalHub.OnRestoreHealth?.Invoke(damage);
                }
            }

            yield return new WaitForSeconds(0.25f);

        }
    }
}
