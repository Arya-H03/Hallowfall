using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWave : MonoBehaviour
{
    [SerializeField] int damage = 30;

    private void Start()
    {

        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().OnEnemyHit(damage, collision.transform.position);
        }
    }
}
