using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyBaseAttack
{
    [SerializeField] private Vector2 boxCastSize = new Vector2(1.75f, 0.5f);
    [SerializeField] Transform boxCastCenter;
    private float distance = 0;
    [SerializeField] LayerMask layerMask;

    [SerializeField] private int parryDamage = 100;

    

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }
    //private void Update()
    //{
    //    DrawCast();
    //}

    public override void HandleAttack()
    {
        Vector2 direction = transform.right;

        RaycastHit2D [] hits = Physics2D.BoxCastAll(new Vector2(boxCastCenter.position.x, boxCastCenter.position.y), boxCastSize, 0f, direction, distance, layerMask);

        AudioManager.Instance.PlaySFX(audioSource, attackSFX[Random.Range(0,attackSFX.Length -1 )]);

         foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider.CompareTag("ParryShield") == true)
            {
                GameObject parryShield = hit.collider.gameObject;
                enemyController.collisionManager.OnEnemyParried(parryShield, hit.point, parryDamage);
                enemyController.ChangeState(EnemyStateEnum.Stun);
                return;
            }
            
        }

        foreach (RaycastHit2D hit in hits)
        {                  
            if (hit.collider.CompareTag("Player"))
            {
                GameObject player = hit.collider.gameObject;
                player.GetComponent<PlayerController>().OnTakingDamage(attackDamage);
                return;
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

    public void DrawCast()
    {
        VisualizeBoxCast(boxCastCenter.position, boxCastSize, transform.right, distance);
    }
}
