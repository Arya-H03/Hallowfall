using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "UmbralWaveSkill", menuName = "Scriptable Objects/Skills/UmbralWaveSkill")]
public class UmbralWaveSkill : BaseSkillSO, IChance, IDamage, ILifeTime,IAreaOfEffect,ISpeed,IPierceCount
{
    [SerializeField] private PlayerProjectile umbralWavePrefab;

    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float chance;
    [SerializeField] protected float effectSize;
    [SerializeField] protected int pierceCount = 1;

    public int Damage { get => damage; set => damage = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }
    public float AreaOfEffect { get => effectSize; set => effectSize = value; }
    public float Chance { get => chance; set => chance = value; }
    public float Speed { get => speed; set => speed = value; }
    public int PierceCount { get => pierceCount; set => pierceCount = value; }

    public override void ApplySkillLogic(PlayerController playerController)
    {
        playerController.PlayerSignalHub.OnSwordAttackHitFrame += () => UmbralWaveLogic(playerController);
        lvl = 1;
    }

    public override void LevelUpSkill(PlayerController playerController)
    {
        lvl++;
        Chance += 0.05f;
        Damage += 10;
    }

    public override string GetSkillDescription()
    {
        return $"Your sword swings have a <color=purple>{Chance * 100 + (5 * lvl)}%</color> chance to unleash an Umbral Wave, dealing <color=red>{Damage + (10 * lvl)}</color> damage to enemies hit.";
    }

    private void UmbralWaveLogic(PlayerController playerController)
    {
        if(!MyUtils.EvaluateChance(Chance)) return;

        PlayerProjectile umbralWave = Instantiate(umbralWavePrefab, playerController.GetPlayerPos(), Quaternion.identity);
        umbralWave.Init(Damage,Speed,LifeTime,PierceCount);
        umbralWave.SetProjectileCourseToCursor();
    }

    
}
