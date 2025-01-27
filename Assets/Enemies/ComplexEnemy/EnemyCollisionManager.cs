using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionManager : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private EnemyController enemyController;

    [SerializeField] float luanchModifier = 1f;

    [SerializeField] DamagePopUp damagePopUp;

    [SerializeField] GameObject impactEffect;

    public bool isInvincible = false;

    public BoxCollider2D BoxCollider { get => boxCollider; set => boxCollider = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        Rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();

    }

    public void LaunchEnemy(Vector2 lanunchVector)
    {
        Rb.velocity = new Vector2(Rb.velocity.x + lanunchVector.x * luanchModifier, Rb.velocity.y + lanunchVector.y * luanchModifier);
    }

    public void OnEnemyParried(GameObject shield, Vector2 hitLocation, int damage)
    {
        shield.GetComponent<ParryShield>().OnSuccessfulParry();
        shield.GetComponent<ParryShield>().SpawnImpactEffect(hitLocation);
        enemyController.OnEnemyHit(damage,hitLocation/*,null*/);
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

    public void SetColliderIsTrigger(bool value)
    {
        BoxCollider.isTrigger = value;
    }

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }
}
