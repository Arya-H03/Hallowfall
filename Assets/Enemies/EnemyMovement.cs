using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //private EnemyAI enemy;
    private EnemyController enemyController;

    private int currentDir = 1;

    private float distanceToTarget;

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }

    private void Awake()
    {
        //enemy = GetComponent<EnemyAI>();
        enemyController = GetComponent<EnemyController>();
    }

    //private void Update()
    //{
    //    distanceToTarget = Mathf.Abs(targetPosition.x - transform.position.x);
    //}
    public void MoveTo(Vector2 startPoint, Vector2 endPoint, float speed)
    {
       
            Vector2 direction = (endPoint - startPoint).normalized;
        //transform.position += new Vector3(direction.x, 0, 0) * speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(startPoint, new Vector2(endPoint.x,startPoint.y), speed * Time.deltaTime);
        TurnEnemy(direction);
            
    
    }

    private void TurnEnemy(Vector2 direction)
    {
        
        if(direction.x < 0 && currentDir != 1)
        {
            currentDir = 1;
            this.transform.localScale = new Vector3(currentDir, 1, 1);   
            //OnEnemyBeginTurning(1);

        }
        if(direction.x >= 0 && currentDir != -1)
        {
            currentDir = -1;
            this.transform.localScale = new Vector3(currentDir, 1, 1);
            //OnEnemyBeginTurning(-1);
        }
    }

    private void OnEnemyBeginTurning(int dir)
    {
        currentDir = dir;
        //enemyController.ChangeState(EnemyStateEnum.Turn);
      
    }

    //Gets called from animation manager in the second to last frame of the turn animation.
    public void OnEnemyEndTurning()
    {
        //transform.localScale = new Vector3(currentDir, 1, 1);
        //enemyController.GetDialogueBox().transform.localScale = new Vector3(currentDir, 1, 1);
        //enemyController.SetIsTurning(false);
        //enemyController.SetCanChangeState(true);
        //enemyController.ChangeState(enemyController.previousStateEnum);
   
    }

    
}
