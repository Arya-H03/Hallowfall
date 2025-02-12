using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionManager : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private EnemyController enemyController;

    private bool canStagger = true;    

    [SerializeField] float luanchModifier = 1f;
    [SerializeField] DamagePopUp damagePopUp;
    [SerializeField] GameObject impactEffect;

    [SerializeField] float maxStagger;
    [SerializeField] float currentStagger;

    [System.Serializable]
    struct HitSFX
    {
        public HitSfxType hitType;
        public AudioClip[] sound;
    }

    [SerializeField] HitSFX[] hitSFXs;
    private Dictionary<HitSfxType, AudioClip[]> hitSFXDictionary;
    public BoxCollider2D BoxCollider { get => boxCollider; set => boxCollider = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }
    public bool CanStagger { get => canStagger; set => canStagger = value; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        Rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        FillDictionary();

    }
 
    private void FillDictionary()
    {
        hitSFXDictionary = new Dictionary<HitSfxType, AudioClip[]>();
        foreach (var hitSfx in hitSFXs)
        {
            hitSFXDictionary[hitSfx.hitType] = hitSfx.sound;
        }
    }
    public IEnumerator EnemyHitCoroutine(int damage, Vector2 hitPoint, HitSfxType hitType)
    {
        //VFX
        PlayBloodEffect(hitPoint);
        enemyController.Material.SetFloat("_Flash", 1);
        //SFX
        AudioManager.Instance.PlaySFX(enemyController.AudioSource, GetHitSound(hitType));
        //Damage
        enemyController.OnEnemyTakingDamage(damage, enemyController.DamageModifier);

        ManageStagger(damage);

        //Wait?
        yield return new WaitForSeconds(0.1f);
        enemyController.Material.SetFloat("_Flash", 0);



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
        if (hitSFXDictionary.TryGetValue(hitType, out AudioClip[] sfx))
        {
            return sfx.Length > 0 ? sfx[Random.Range(0, sfx.Length)] : null;

        }
        return null;
    }

    private void ManageStagger(float damage)
    {
        if (currentStagger < maxStagger && canStagger) 
        {
            //Turn damage to stagger amount
            float amount = 2*damage;
            currentStagger += amount;
            if (currentStagger >= maxStagger)
            {
                //stun enemy
                StartCoroutine(DelayStaggerCoroutine());
                enemyController.ChangeState(EnemyStateEnum.Stun);
            }
        }
     

    }

    private IEnumerator DelayStaggerCoroutine()
    {
        canStagger = false;
        yield return new WaitForSeconds(enemyController.StunState.StunDuration + 2);
        canStagger = true;
    }

    public void ResetStagger()
    {
        currentStagger = 0;
    }
    public void LaunchEnemy(Vector2 lanunchVector)
    {
        Rb.velocity = new Vector2(Rb.velocity.x + lanunchVector.x * luanchModifier, Rb.velocity.y + lanunchVector.y * luanchModifier);
    }

    public void OnEnemyParried(GameObject shield, Vector2 hitLocation, int damage)
    {
        shield.GetComponent<ParryShield>().OnSuccessfulParry();
        shield.GetComponent<ParryShield>().SpawnImpactEffect(hitLocation);
        enemyController.OnEnemyHit(damage,hitLocation, HitSfxType.sword);
        Vector3 scale = transform.localScale;
        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(5 * luanchModifier, 3 * luanchModifier);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-5 * luanchModifier, 3 * luanchModifier);
        }

        //LaunchEnemy(launchVec);
    }
    public void ApplyVelocity(float x, float y)
    {
        Rb.velocity = new Vector2(x, y);
    }
    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    public override bool Equals(object obj)
    {
        return obj is EnemyCollisionManager manager &&
               base.Equals(obj) &&
               CanStagger == manager.CanStagger;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(base.GetHashCode(), CanStagger);
    }
}
