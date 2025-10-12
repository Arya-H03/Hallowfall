using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "DecimationSkill", menuName = "Scriptable Objects/Skills/DecimationSkill")]
public class DecimationSkill : BaseSkillSO, IFloatRange
{
    [SerializeField] private float lowerEnd;
    [SerializeField] private float upperEnd;
    public float LowerEnd { get => lowerEnd; set => lowerEnd = value; }
    public float UpperEnd { get => upperEnd; set => upperEnd = value; }

    public override void ApplySkillLogic(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyHit += DecimationLogic;
        lvl = 1;
    }

    public override void LevelUpSkill(PlayerController playerController)
    {
        lvl++;
        LowerEnd += 0.02f;
        UpperEnd += 0.02f;
    }
    public override string GetSkillDescription()
    {
        if (lvl == 0) return $"Your first damaging sword attack on an enemy will deal an additional <color=red>{LowerEnd * 100 }%</color> to <color=red>{UpperEnd * 100}%</color> of the enemies max health";
        else return $"Your first damaging sword attack on an enemy will deal an additional <color=red>{LowerEnd * 100 + 2}%</color> to <color=red>{UpperEnd * 100 + 2}%</color> of the enemies max health";
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
