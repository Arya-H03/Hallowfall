using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySightHandler : ActiveAbilityHandler
{
    public List<GameObject> nearbyEnemies = new List<GameObject>();
    public List<GameObject> markedEnemies = new List<GameObject>();

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float searchDelay = 5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float damageModifier = 2f;
    [SerializeField] private float speedModifier = 0;

    [SerializeField] private Material markOutline;

    private void Start()
    {
        StartCoroutine(FindNearbyEnemies());
    }

    private IEnumerator FindNearbyEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(searchDelay);

            // Clear the list before updating
            nearbyEnemies.Clear();

            RaycastHit2D[] enemies = Physics2D.CircleCastAll(transform.position, detectionRadius, Vector2.zero, 0, enemyLayer);

            foreach (RaycastHit2D enemy in enemies)
            {
                if (enemy.collider.CompareTag("Enemy"))
                {
                    nearbyEnemies.Add(enemy.transform.gameObject);
                }
            }

            if (nearbyEnemies.Count > 0)
            {
                ApplyMark();
            }
        }
    }

    private void ApplyMark()
    {
        // Filter out already marked enemies
        List<GameObject> unmarkedEnemies = nearbyEnemies.FindAll(e => !markedEnemies.Contains(e));

        if (unmarkedEnemies.Count > 0)
        {
            int index = Random.Range(0, unmarkedEnemies.Count);
            GameObject selectedEnemy = unmarkedEnemies[index];

            markedEnemies.Add(selectedEnemy);
            selectedEnemy.GetComponent<SpriteRenderer>().material = markOutline;
            selectedEnemy.GetComponent<EnemyController>().DamageModifier *= damageModifier;
            selectedEnemy.GetComponentInChildren<ChaseState>().ChaseSpeed *= (1-speedModifier);
            
        }
    }
}
