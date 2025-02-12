using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    private int damage;
    private Vector2 hitPoint;
    private HitSfxType hitType;
    [SerializeField] private float hitDelay = 0.5f;

    [System.Serializable]
    struct HitSFX
    {
        public HitSfxType hitType;
        public AudioClip [] sound;
    }

    [SerializeField] HitSFX[] hitSFXs;
    private Dictionary<HitSfxType, AudioClip[]> hitSFXDictionary;
    public int Damage { get => damage; set => damage = value; }
    public Vector2 HitPoint { get => hitPoint; set => hitPoint = value; }
    public HitSfxType HitType { get => hitType; set => hitType = value; }

    private void Awake()
    {
        hitSFXDictionary = new Dictionary<HitSfxType, AudioClip[]>();
        foreach(var hitSfx in hitSFXs)
        {
            hitSFXDictionary[hitSfx.hitType] = hitSfx.sound;
        }
    }
    public EnemyHitState() : base()
    {
        stateEnum = EnemyStateEnum.Hit;

    }
    public override void OnEnterState()
    {
        StartCoroutine(EnemyHitCoroutine(damage, hitPoint, hitDelay, HitType));
    }

    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {


    }

    private IEnumerator EnemyHitCoroutine(int damage, Vector2 hitPoint, float delay, HitSfxType hitType)
    {
        //VFX
        PlayBloodEffect(hitPoint);
        enemyController.Material.SetFloat("_Flash", 1);
        //SFX
        AudioManager.Instance.PlaySFX(enemyController.AudioSource, GetHitSound(hitType));
        //Damage
        enemyController.OnEnemyTakingDamage(damage, enemyController.DamageModifier);

        //Wait?
        yield return new WaitForSeconds(delay);
        enemyController.Material.SetFloat("_Flash", 0);
        enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    private void PlayBloodEffect(Vector2 hitPoint)
    {
        Vector2 distance = hitPoint - new Vector2(this.transform.position.x, this.transform.position.y);
        var shape = enemyController.BloodParticles.shape;
        shape.position = (distance);
        enemyController.BloodParticles.Play();
    }

    private AudioClip GetHitSound(HitSfxType hitType)
    {
        if(hitSFXDictionary.TryGetValue(hitType,out AudioClip[] sfx))
        {
            return sfx.Length >0 ? sfx[Random.Range(0,sfx.Length)] : null;   

        }
        return null;
    }

}
