using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private Material material;
    private EnemyAI enemyAi;
    private Enemy enemy;
    private Rigidbody2D rb;
    public float luanchModifier;
    [SerializeField] DamagePopUp damagePopUp;
    public bool isInvincible = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        enemyAi = GetComponent<EnemyAI>();
        material = GetComponent<SpriteRenderer>().material;

    }

    public void OnEnemyHit(Vector2 lanunchVector, int damage)
    {
        if (!isInvincible)
        {
            enemyAi.ManageStunValue(damage);
            StartCoroutine(EnemyFlashOnHit());
            SpawnDamagePopUp(damage);
            enemy.OnEnemyDamage(damage);
            LaunchEnemy(lanunchVector);
        }
        
        

    }

    private IEnumerator EnemyFlashOnHit()
    {
        material.SetFloat("_Flash", 1);
        yield return new WaitForSeconds(0.25f);
        material.SetFloat("_Flash", 0);
    }
    private void LaunchEnemy(Vector2 lanunchVector)
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
}
