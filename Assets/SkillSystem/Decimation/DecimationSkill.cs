using UnityEngine;

[CreateAssetMenu(fileName = "DecimationSkill", menuName = "Scriptable Objects/Skills/DecimationSkill")]
public class DecimationSkill : BaseSkillSO, IFloatRange
{
    [SerializeField] private float lowerEnd;
    [SerializeField] private float upperEnd;
    public float LowerEnd { get => lowerEnd; set => lowerEnd = value; }
    public float UpperEnd { get => upperEnd; set => upperEnd = value; }

    public override void Init(EntityController controller)
    {
        if (controller == null) return;
        if (controller is PlayerController playerController)
        {
            playerController.PlayerSignalHub.OnEnemyHit += DecimationLogic;
            Debug.Log(abilityName);
        }
    }
    private void DecimationLogic(EnemyController enemyController, int swordHitDmage)
    {
        IDamagable damagable = enemyController.GetComponent<IDamagable>();
        if(swordHitDmage + damagable.CurrentHealth >= damagable.MaxHealth)
        {
            float rand = Random.Range(LowerEnd, UpperEnd);

            damagable.ApplyDamage((int)(damagable.MaxHealth * rand));
        }
    }

}
