using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KnightAttack : MonoBehaviour
{
    //[SerializeField] Vector2 size; // Size of the box in 2D
    //[SerializeField] Transform loc; // Distance for the boxcast in 2D
    [SerializeField] LayerMask layerMask; // Layer mask for the boxcast; // Distance for the boxcast in 2D
    private float distance = 0; // Distance for the boxcast in 2D

    [SerializeField] Vector2 lightAttackSize; 
    [SerializeField] Transform lightAttackPoint;

    [SerializeField] Vector2 heavyAttackSize;
    [SerializeField] Transform heavyAttackPoint;

    [SerializeField] int lightAttackDamage = 20;
    [SerializeField] int HeavyAttackDamage = 50;

    [SerializeField] int parryDamage = 100;






    private KnightAI knightAI;
    private void Awake()
    {
        knightAI = GetComponent<KnightAI>();
    }
    public void LightAttackBoxCast()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(lightAttackPoint.position.x, lightAttackPoint.position.y), lightAttackSize, 0f, direction, distance, layerMask);

        if (hit)
        {
            if (hit.collider.CompareTag("ParryShield") == true)
            {

                GameObject parryShield = hit.collider.gameObject;
                knightAI.enemyCollision.OnEnemyParried(parryShield, hit.point, parryDamage);

            }


            if (hit.collider.CompareTag("Player"))
            {


                GameObject player = hit.collider.gameObject;
                player.GetComponent<PlayerController>().OnTakingDamage(lightAttackDamage);


            }
        }

       

    }

    public void HeavyAttackBoxCast()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(heavyAttackPoint.position.x, heavyAttackPoint.position.y), heavyAttackSize, 0f, direction, distance, layerMask);

        if (hit.collider.CompareTag("ParryShield") == true)
        {

            GameObject parryShield = hit.collider.gameObject;
           
            knightAI.enemyCollision.OnEnemyParried(parryShield, hit.point,100);

        }


        if (hit.collider.CompareTag("Player"))
        {


            GameObject player = hit.collider.gameObject;
            player.GetComponent<PlayerController>().OnTakingDamage(HeavyAttackDamage);
            //player.GetComponent<Player>().OnKnockBack(new Vector2(10, 10),this.transform.position.x);


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
        //VisualizeBoxCast(loc.position, size, transform.right, distance);
    }


}
