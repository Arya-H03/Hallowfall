using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class SwordAttack : MonoBehaviour
{
    //[SerializeField] Vector2 size = new Vector2(1.75f, 0.5f);// Size of the box in 2D
    //[SerializeField] Transform loc; // Distance for the boxcast in 2D
    private EnemyController enemyStatesManager;

    private Vector2 swordAttackSize = new Vector2(2f, 0.5f);
    [SerializeField] Transform swordAttackPoint;

    [SerializeField] LayerMask layerMask;

    [SerializeField] private int swordAttackDamage = 0;

    private int parryDamage = 100;

    private float distance = 0;

    private void Awake()
    {
        enemyStatesManager = GetComponentInParent<EnemyController>();    
    }
    public void SwordAttackBoxCast()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(swordAttackPoint.position.x, swordAttackPoint.position.y), swordAttackSize, 0f, direction, distance, layerMask);

        if (hit)
        {
         

            if (hit.collider.CompareTag("ParryShield") == true)
            {

                GameObject parryShield = hit.collider.gameObject;
                enemyStatesManager.collisionManager.OnEnemyParried(parryShield, hit.point, parryDamage);
                enemyStatesManager.ChangeState(EnemyStateEnum.Stun);
              
            }

            if (hit.collider.CompareTag("Player"))
            {
                GameObject player = hit.collider.gameObject;
                player.GetComponent<PlayerController>().OnTakingDamage(swordAttackDamage);
            }
        }

    }

    private void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    {
        // Define the corners of the box for visualization in 2D
        Vector2 topLeft = origin + (Vector2.left * size.x / 2) + (Vector2.up * size.y / 2);
        Vector2 topRight = origin + (Vector2.right * size.x / 2) + (Vector2.up * size.y / 2);
        Vector2 bottomLeft = origin + (Vector2.left * size.x / 2) + (Vector2.down * size.y / 2);
        Vector2 bottomRight = origin + (Vector2.right * size.x / 2) + (Vector2.down * size.y / 2);

        // Draw the edges of the box using Debug.DrawLine for visualization in 2D
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        // Draw the ray from the center to the right (assuming right is forward) for visualization in 2D
        Debug.DrawRay(origin, direction * distance, Color.red);
    }

    //public void DrawCast()
    //{
    //    VisualizeBoxCast(loc.position, size, transform.right, distance);
    //}

}
