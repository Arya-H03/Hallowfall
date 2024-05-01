using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //private EnemyAI enemy;
    private EnemyStatesManager statesManager;

    private int currentDir = 1;
    private bool isTurning = false; 

    private void Awake()
    {
        //enemy = GetComponent<EnemyAI>();
        statesManager = GetComponent<EnemyStatesManager>();
    }

    public void MoveTo(Vector2 startPoint, Vector2 endPoint, float speed)
    {
        if (!isTurning)
        {
            Vector2 direction = endPoint - startPoint;
            transform.position = Vector2.MoveTowards(startPoint, endPoint, speed * Time.deltaTime);
            TurnEnemy(direction);
            //enemy.rb.velocity = direction * enemySpeed;
        }

    }

    private void TurnEnemy(Vector2 direction)
    {
        
        if(direction.x < 0 && currentDir != 1)
        {
            OnEnemyBeginTurning(1);
        }
        if(direction.x >= 0 && currentDir != -1)
        {
            OnEnemyBeginTurning(-1);
        }
    }

    private void OnEnemyBeginTurning(int dir)
    {
        currentDir = dir;
        isTurning = true;

        statesManager.animationManager.SetBoolForAnimation("isTurning", isTurning);
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
    }
    public void OnEnemyEndTurning(bool value)
    {
        isTurning = value;
        statesManager.animationManager.SetBoolForAnimation("isTurning", isTurning);
        transform.localScale = new Vector3(currentDir, 1, 1);
        statesManager.animationManager.SetBoolForAnimation("isRunning", true);
    }
}
