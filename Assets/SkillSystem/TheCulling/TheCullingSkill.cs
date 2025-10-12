using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "TheCullingSkill", menuName = "Scriptable Objects/Skills/TheCullingSkill")]
public class TheCullingSkill : BaseSkillSO, ITreshold
{
    [SerializeField] private float cullingTreshold; 
    public float Treshold { get => cullingTreshold; set => cullingTreshold = value; } 
    public override void ApplySkillLogic(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnEnemyHit += TheCullingLogic;
      
        lvl = 1;
    }

    public override void LevelUpSkill(PlayerController playerController)
    {
        lvl++;
        Treshold += 0.05f;
       
    }
    public override string GetSkillDescription()
    {
        if(lvl == 0) return $"Once an enemy reaches <color=orange>{Treshold * 100}%</color> health, your next sword attack will instantly kill the enemy regardless of the remaining health";
        else return $"Once an enemy reaches <color=orange>{Treshold * 100 + (5)}%</color> health, your next sword attack will instantly kill the enemy regardless of the remaining health";

    }
    private void TheCullingLogic(EnemyController enemyController, int swordHitDmage)
    {
        IDamagable damagable = enemyController.GetComponent<IDamagable>();
        float enemyHealthRatio = (float)damagable.CurrentHealth / damagable.MaxHealth;
        
        if (enemyHealthRatio < Treshold)
        {
           
            Debug.Log("Cull");
            damagable.ApplyDamage(damagable.MaxHealth);
        }
    }



    
}
