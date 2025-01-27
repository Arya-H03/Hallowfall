using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : MonoBehaviour
{
    [SerializeField] int damage = 20;

    private void Start()
    {
        Destroy(gameObject,0.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().OnEnemyHit(damage, collision.transform.position);
        }
    }
}
