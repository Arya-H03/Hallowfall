using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeBox : MonoBehaviour
{
    private EnemyAI enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.OnEnterChaseState();
        }
    }
    

}
