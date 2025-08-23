
using System.Collections;
using UnityEngine;

public class PlayerParryShield : MonoBehaviour
{
    private PlayerController playerController;
    private BoxCollider2D boxCollider;

    public BoxCollider2D BoxCollider { get => boxCollider;}

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerController = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyProjectile") && playerController.StateMachine.PlayerParryState.CanParryProjectiles)
        {
            StartCoroutine(BladeReflect(collision.gameObject));
        }
    }
    private IEnumerator BladeReflect(GameObject proj)
    {
        EnemyProjectile enemyProjectile = proj.GetComponent<EnemyProjectile>();
        if (enemyProjectile == null) yield break;

        int damage = enemyProjectile.Damage;
        float speed = enemyProjectile.Speed;
        float lifeTime = enemyProjectile.LifeTime;
        GameObject enemy = enemyProjectile.EnemyOwner;

        Destroy(enemyProjectile);
        yield return null; // Wait 1 frame

        PlayerProjectiles playerProjectiles = proj.AddComponent<PlayerProjectiles>();
        playerProjectiles.Damage = damage;
        playerProjectiles.Speed = speed;
        playerProjectiles.LifeTime = lifeTime;
        playerProjectiles.SetProjectileCourseToTarget(enemy);
    }

}
