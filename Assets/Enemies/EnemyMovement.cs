using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //private EnemyAI enemy;

    private void Awake()
    {
        //enemy = GetComponent<EnemyAI>();
    }

    public void MoveTo(Vector2 startPoint, Vector2 endPoint, float speed)
    {
        Vector2 direction = endPoint - startPoint;
        transform.position = Vector2.MoveTowards(startPoint, endPoint, speed * Time.deltaTime);
        TurnEnemy(direction);
        //enemy.rb.velocity = direction * enemySpeed;
    }

    private void TurnEnemy(Vector2 direction)
    {
        if(direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if(direction.x >= 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
