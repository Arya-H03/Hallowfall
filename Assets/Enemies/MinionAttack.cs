using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAttack : MonoBehaviour
{
    

    [SerializeField] Vector2 size; // Size of the box in 2D
    [SerializeField] float distance; // Distance for the boxcast in 2D
    [SerializeField] Transform loc; // Distance for the boxcast in 2D
    [SerializeField] LayerMask layerMask; // Layer mask for the boxcast; // Distance for the boxcast in 2D
    [SerializeField] int parryDamage = 50; // Layer mask for the boxcast; // Distance for the boxcast in 2D


    private EnemyAI enemyAI;
    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
    }
    public void BoxCast()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(loc.position.x, loc.position.y), size, 0f, direction, distance, layerMask);

        if (hit)
        {
            if (hit.collider.CompareTag("ParryShield") == true)
            {

                GameObject parryShield = hit.collider.gameObject;
                parryShield.GetComponent<ParryShield>().OnSuccessfulParry();
                parryShield.GetComponent<ParryShield>().SpawnImpactEffect(hit.point);
                Vector3 scale = transform.localScale;

                enemyAI.enemyCollision.OnEnemyParried(parryShield, hit.point, parryDamage);


            }


            if (hit.collider.CompareTag("Player"))
            {


                GameObject player = hit.collider.gameObject;
                player.GetComponent<PlayerController>().OnTakingDamage(50);


            }
        }
       
        
    }

    void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
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

    private void Update()
    {
        VisualizeBoxCast(loc.position, size, transform.right, distance);
    }


}
