using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] GameObject[] bloofVFX;

    private float maxStagger;
    private float currentStagger = 0;

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
        enemyController = GetComponentInParent<EnemyController>();
        Rb = GetComponentInParent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

        FillDictionary();

    }

    private void Start()
    {
        maxStagger = enemyController.EnemyConfig.maxStagger;
    }
    private void FillDictionary()
    {
        hitSFXDictionary = new Dictionary<HitSfxType, AudioClip[]>();
        foreach (var hitSfx in hitSFXs)
        {
            hitSFXDictionary[hitSfx.hitType] = hitSfx.sound;
        }
    }


    public AudioClip GetHitSound(HitSfxType hitType)
    {
        if (hitSFXDictionary.TryGetValue(hitType, out AudioClip[] sfx))
        {
            return sfx.Length > 0 ? sfx[Random.Range(0, sfx.Length)] : null;

        }
        return null;
    }

    public bool TryStagger(float damageTaken)
    {
        if (currentStagger < maxStagger && canStagger)
        {
            currentStagger += (2* damageTaken);

            if (currentStagger >= maxStagger)
            {
                currentStagger = 0;
                canStagger = false;

                StartCoroutine(EnemyStaggerCoroutine());               
            }
        }

        Debug.Log(canStagger);
        return canStagger;
    }

    private IEnumerator EnemyStaggerCoroutine()
    {
      
        enemyController.ChangeState(EnemyStateEnum.Stun);

        yield return new WaitForSeconds(enemyController.EnemyConfig.timeBetweenStaggers);
        canStagger = true;
    }

 

    public void KnockBackEnemy(Vector2 lanunchVector, float lunchForce)
    {
        StartCoroutine(KnockBackEnemyCoroutine(lanunchVector, lunchForce));
    }
    private IEnumerator KnockBackEnemyCoroutine(Vector2 lanunchVector, float force)
    {
        enemyController.CanMove = false;
        enemyController.IsBeingknocked = true;
        enemyController.StunState.StunDuration = 1f;
        enemyController.ChangeState(EnemyStateEnum.Stun);
        Rb.linearVelocity += lanunchVector * luanchModifier * force;
            
        yield return new WaitForSeconds(0.25f);
        enemyController.CanMove = true;
        enemyController.IsBeingknocked = false;
        Rb.linearVelocity -= lanunchVector * luanchModifier * force;
        

    }

    public void OnEnemyParried(GameObject shield, Vector2 hitLocation, int damage)
    {
        PlayerParryState parryState = shield.GetComponentInParent<PlayerParryState>();
        parryState.SpawnImpactEffect(hitLocation);

        if (parryState.CanCounter())
        {
            parryState.CallOnParrySuccessfulEvent();
            enemyController.HitEnemy(damage, hitLocation, HitSfxType.sword,2);
            Vector3 scale = transform.localScale;
         
        }

    }
    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

  
}
