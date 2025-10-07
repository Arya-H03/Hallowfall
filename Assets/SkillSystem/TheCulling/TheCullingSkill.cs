using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "TheCullingSkill", menuName = "Scriptable Objects/Skills/TheCullingSkill")]
public class TheCullingSkill : BaseSkillSO, ITreshold
{
    [SerializeField] private float cullingTreshold; 
    public float Treshold { get => cullingTreshold; set => cullingTreshold = value; }
   
    private EntityController ownerEntity;

 
    public override void Init(EntityController controller)
    {
        if (controller == null) return;
        ownerEntity = controller;
        if (ownerEntity is PlayerController playerController)
        {
            playerController.PlayerSignalHub.OnEnemyHit += TheCullingLogic;
            Debug.Log(abilityName);
        }
    }

    public override string GetDescription()
    {
        return $"Once an enemy reaches <color=orange>{Treshold*100}%</color>, your next sword attack will instantly kill the enemy regardless of the remaining health";
    }
    private void TheCullingLogic(EnemyController enemyController, int swordHitDmage)
    {
        IDamagable damagable = enemyController.GetComponent<IDamagable>();
        float enemyHealthRatio = damagable.CurrentHealth / damagable.MaxHealth;
        if (enemyHealthRatio < Treshold)
        {
            damagable.ApplyDamage(damagable.MaxHealth);
        }
    }
}
