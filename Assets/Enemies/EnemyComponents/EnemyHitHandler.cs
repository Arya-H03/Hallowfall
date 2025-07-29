using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class EnemyHitHandler : MonoBehaviour,IDamagable,IInitializeable<EnemyController>
{
    private EnemyController enemyController;
    private EnemyPhysicsHandler physicsHandler;
    private EnemyAnimationHandler animationHandler;
    private EnemyHealthbarHandler healthbarHandler;
    private EnemyConfigSO config;

    private Coroutine squashCoroutine;
    private Vector3 originalScale;
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.physicsHandler = enemyController.EnemyPhysicsHandler;
        this.animationHandler =enemyController.EnemyAnimationHandler;
        this.healthbarHandler = enemyController.EnemyHealthbarHandler;
        config = enemyController.EnemyConfig;

        MaxHealth = enemyController.EnemyConfig.maxHealth;
        CurrentHealth = MaxHealth;
        DamageModifier = enemyController.EnemyConfig.damageModifier;
        originalScale = enemyController.transform.localScale;
    }
    public void HitEnemy(float damageAmount, HitSfxType hitType, float knockbackForce)
    {
        physicsHandler.TryStagger(damageAmount);
        animationHandler.SetTriggerForAnimation("Hit");

        PlayBloodEffect();
        if (squashCoroutine == null)
        {
            squashCoroutine = StartCoroutine(SquashEnemyCoroutine(animationHandler.HitAnimDuration * 0.75f));
        }
        else
        {
            StopCoroutine(squashCoroutine);
            enemyController.transform.localScale = originalScale;
            StartCoroutine(SquashEnemyCoroutine(animationHandler.HitAnimDuration * 0.75f));

        }
        

        if (hitType != HitSfxType.none)
            AudioManager.Instance.PlaySFX(physicsHandler.GetHitSound(hitType), transform.position, 0.4f);

        ApplyDamage(damageAmount * DamageModifier);
        SpawnDamagePopUp(damageAmount * DamageModifier);
        healthbarHandler.UpdateEnemyHealthBar();
    }

    private IEnumerator SquashEnemyCoroutine(float duration)
    {
        float ySquish = Random.Range(-0.075f, 0.075f);
        float xSquish = -ySquish + Random.Range(-0.035f, 0.035f);
        float halfDuration = duration / 2f;
        float timer = 0;

        Vector3 originScale = transform.localScale;
        Vector3 squashedScale = originScale - new Vector3(xSquish, ySquish, 0);


        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.localScale = Vector3.Lerp(originScale, squashedScale, t);
            yield return null;
        }

        transform.localScale = squashedScale;


        timer = 0;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            float t = timer / halfDuration;
            transform.localScale = Vector3.Lerp(squashedScale, originScale, t);
            yield return null;
        }
        transform.localScale = originScale;
    }
    private void PlayBloodEffect()
    {
        Vector3 randPos = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
        GameObject go = Instantiate(config.bloofVFXPrefabs[Random.Range(0, config.bloofVFXPrefabs.Length)], enemyController.GetEnemyPos() + randPos, Quaternion.identity);
        Vector3 scale = go.transform.localScale;

        int randX = (int)MyUtils.GetRandomValue<int>(new int[] { -1, 1 });
        scale.x *= randX;
        go.transform.localScale = scale;
    }


    //public IEnumerator EnemyHitCoroutine(float damageAmount, Vector2 hitPoint, HitSfxType hitType, float knockbackForce)
    //{     
    //    //material.SetFloat("_Flash", 1);   
    //    yield return new WaitForSeconds(0.1f);
    //    //material.SetFloat("_Flash", 0);
    //}

    public void ApplyDamage(float amount)
    {
        if (enemyController.IsDead) return;

        float damage = amount * DamageModifier;
        CurrentHealth -= damage;

        if (CurrentHealth <= 0) Die();
    }

    public void RestoreHealth(float amount)
    {
        CurrentHealth = amount;
    }

    private void SpawnDamagePopUp(float damage)
    {
        var obj = Instantiate(config.damagePopUpPrefab, transform.position + Vector3.up, Quaternion.identity);
        obj.SetText(damage.ToString());
    }

    public void Die()
    {
        CurrentHealth = 0;
        enemyController.EnemyStateMachine.ChangeState(EnemyStateEnum.Death);
    }
}
