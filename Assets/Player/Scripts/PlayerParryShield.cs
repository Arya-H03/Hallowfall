
using System.Collections;
using UnityEngine;

public class PlayerParryShield : MonoBehaviour
{
    private PlayerParryState parryState;

    private void Awake()
    {
        parryState = GetComponentInParent<PlayerParryState>();
    }
    private void OnEnable()
    {
        SkillEvents.OnBladeReflectionSkillUnlocked += BladeReflectionSkillLogic;
    }
    private void OnDisable()
    {
        SkillEvents.OnBladeReflectionSkillUnlocked -= BladeReflectionSkillLogic;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyProjectile") && parryState.CanParryProjectiles)
        {
            StartCoroutine(BladeReflect(collision.gameObject));
        }
    }

    private void BladeReflectionSkillLogic()
    {
        parryState.CanParryProjectiles = true;
    }

    private IEnumerator BladeReflect(GameObject proj)
    {
        EnemyProjectile enemyProjectile = proj.GetComponent<EnemyProjectile>();
        if (enemyProjectile == null) yield break;

        float damage = enemyProjectile.Damage;
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
