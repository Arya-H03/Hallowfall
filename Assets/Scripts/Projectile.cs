using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private int damage = 10;
    private Rigidbody2D rb;

    private EnemyController enemyController;

    public EnemyController EnemyController { get => enemyController; set => enemyController = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(this, 5);
    }

    public void SetVelocity(Vector3 targetPositon)
    {
        Vector3 dir = (targetPositon - this.transform.position).normalized;

        rb.velocity = dir * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().OnTakingDamage(damage);
            if((collision.transform.position - this.transform.position).x < 0)
            {
                collision.GetComponent<PlayerCollision>().KnockPlayer(collision.transform.right * -1 * 5);
                
            }
            else
            {
                collision.GetComponent<PlayerCollision>().KnockPlayer(collision.transform.right * 1 * 5);
                
            }
            
            Destroy(this.gameObject);
        }

        if (collision.CompareTag("ParryShield"))
        {
            SetVelocity(EnemyController.transform.position);
        }

        if (collision.CompareTag("Enemy"))
        {
            //if ((collision.transform.position - this.transform.position).x < 0)
            //{
            //    collision.GetComponent<Ene>().OnEnemyHit(collision.transform.right * -1 * 5, damage);
            //    collision.GetComponent<EnemyController>().PlayBloodEffect(EnemyController.transform.position);

            //}
            //else
            //{
            //    collision.GetComponent<EnemyCollisionManager>().OnEnemyHit(collision.transform.right * 1 * 5, damage);
            //    collision.GetComponent<EnemyController>().PlayBloodEffect(EnemyController.transform.position);

            //}

            Destroy(this.gameObject);

        }
    }
}
