using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    private float visionAngle = 90f; 
    private float visionRange = 5f;

    private Transform enemyTransform;

    private EnemyController enemyController;

    private void Awake()
    {
        enemyTransform = transform.parent;
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void FixedUpdate()
    {
        if (!enemyController.hasSeenPlayer && !enemyController.IsPlayerDead)
        {
            // Calculate the start direction of the cone of vision
            Vector2 startDirection = Quaternion.Euler(0f, 0f, -visionAngle / 2f) * new Vector3(-enemyTransform.localScale.x, 0, 0);

            // Cast rays within the cone of vision
            for (float angle = 0f; angle <= visionAngle; angle += 5f)
            {
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * startDirection;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionRange, targetLayer);

                // Check for collisions with the player or other objects
                if (hit.collider != null)
                {

                    if (hit.collider.CompareTag("Player"))
                    {
                        enemyController.player = hit.collider.gameObject;
                        enemyController.hasSeenPlayer = true;
                        enemyController.ChangeState(EnemyStateEnum.Chase);
                    }
                }

                // Visualize the cone of vision 
                //Debug.DrawRay(transform.position, direction * visionRange, Color.red);

            }
        }

    }


}
