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


    public void PlayBloodEffect(Vector2 hitPoint)
    {
        Vector2 distance = hitPoint - new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 dir = FindEffectDir(hitPoint);
        Vector3 center = new Vector3(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
        GameObject go = Instantiate(bloofVFX[Random.Range(0, bloofVFX.Length)], hitPoint, Quaternion.identity);
        Vector3 scale = go.transform.localScale;
        scale.x *= dir.x;
        go.transform.localScale = scale;
    }

    private Vector2 FindEffectDir(Vector3 pointOfImpact)
    {
        Vector3 center = new Vector3(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
        Vector3 dist = (center - pointOfImpact).normalized;

        float xDir = dist.x >= 0 ? -1 : 1;
        float yDir = dist.y >= 0 ? 1 : -1;

        return new Vector2(xDir, yDir);
    }

    public AudioClip GetHitSound(HitSfxType hitType)
    {
        if (hitSFXDictionary.TryGetValue(hitType, out AudioClip[] sfx))
        {
            return sfx.Length > 0 ? sfx[Random.Range(0, sfx.Length)] : null;

        }
        return null;
    }

    public void StaggerEnemy(float damage)
    {
        if (currentStagger < maxStagger && canStagger)
        {
            //Turn damage to stagger amount
            float amount = 2 * damage;
            currentStagger += amount;
            enemyController.StunState.StunDuration = 3f;
            if (currentStagger >= maxStagger)
            {

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
            enemyController.OnEnemyHit(damage, hitLocation, HitSfxType.sword,2);
            Vector3 scale = transform.localScale;
            //Vector2 launchVec = Vector2.zero;
            //if (scale.x == 1)
            //{
            //    launchVec = new Vector2(5 * luanchModifier, 3 * luanchModifier);
            //}
            //if (scale.x == -1)
            //{
            //    launchVec = new Vector2(-5 * luanchModifier, 3 * luanchModifier);
            //}
            //KnockBackPlayer(launchVec);
        }

    }
    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }


}
