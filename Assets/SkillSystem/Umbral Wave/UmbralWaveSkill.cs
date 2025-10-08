using UnityEngine;

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

    public override string GetDescription()
    {
        return $"Your sword swings have a <color=purple>{Chance * 100}%</color> chance to unleash an Umbral Wave, dealing <color=red>{Damage}</color> damage to enemies hit.";
    }

    public override void Init(PlayerController controller)
    {
        if (controller == null) return;
        controller.PlayerSignalHub.OnSwordAttackHitFrame += () => UmbralWaveLogic(controller);
    }

    private void UmbralWaveLogic(PlayerController playerController)
    {
        if(!MyUtils.EvaluateChance(Chance)) return;

        PlayerProjectile umbralWave = Instantiate(umbralWavePrefab, playerController.GetPlayerPos(), Quaternion.identity);
        umbralWave.Init(Damage,Speed,LifeTime,PierceCount);
        umbralWave.SetProjectileCourseToCursor();
    }
}
