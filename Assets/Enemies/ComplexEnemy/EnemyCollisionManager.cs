using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionManager : MonoBehaviour
{
    private Material material;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private EnemyController statesManager;

    [SerializeField] float luanchModifier = 1f;

    [SerializeField] DamagePopUp damagePopUp;

    [SerializeField] GameObject impactEffect;

    public bool isInvincible = false;

    private void Awake()
    {
        statesManager = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        material = GetComponent<SpriteRenderer>().material;

    }

    public void OnEnemyHit(Vector2 lanunchVector, int damage)
    {
        if (!isInvincible)
        {         
            StartCoroutine(HandleEnemyGettingHit(lanunchVector,damage));
        }

    }

    private IEnumerator HandleEnemyGettingHit(Vector2 lanunchVector, int damage)
    {
        isInvincible = true;
        material.SetFloat("_Flash", 1);
        SpawnDamagePopUp(damage);
        statesManager.OnEnemyDamage(damage);
        LaunchEnemy(lanunchVector);
        yield return new WaitForSeconds(0.25f);
        material.SetFloat("_Flash", 0);
        isInvincible = false;
    }
    public void LaunchEnemy(Vector2 lanunchVector)
    {
        rb.velocity = new Vector2(rb.velocity.x + lanunchVector.x * luanchModifier, rb.velocity.y + lanunchVector.y * luanchModifier);
    }

    public void OnEnemyParried(GameObject shield, Vector2 hitLocation, int damage)
    {
        shield.GetComponent<ParryShield>().OnSuccessfulParry();
        shield.GetComponent<ParryShield>().SpawnImpactEffect(hitLocation);
        Vector3 scale = transform.localScale;
        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(7 * luanchModifier, 5 * luanchModifier);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-7 * luanchModifier, 5 * luanchModifier);
        }
        OnEnemyHit(launchVec, damage);
    }

    private void SpawnDamagePopUp(int damage)
    {
        DamagePopUp obj = Instantiate(damagePopUp, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
        obj.SetText(damage.ToString());
    }

    public void ApplyVelocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }

    public void SetColliderIsTrigger(bool value)
    {
        boxCollider.isTrigger = value;
    }

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }
}
