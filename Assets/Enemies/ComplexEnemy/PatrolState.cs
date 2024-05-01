using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Barracuda;
using UnityEngine;
using static EnemyAI;

public class PatrolState : EnemyBaseState
{
    private Vector3 nextPatrollPosition;
    private Vector3 startPosition;
    private float patrolSpeed = 1f;

    private float patrolDelay = 0f;
    private float patrolDelayTimer = 0f;
    private int patrolDirection = 1;

    public PatrolState() : base()
    {
        stateEnum = EnemyStateEnum.Patrol;
    }
    private void Awake()
    {
        
    }

    private void Start()
    {
        startPosition = this.transform.position;
        SetNextPatrolPoint();
    }
    public override void OnEnterState()
    {
        SetNextPatrolPoint();
        statesManager.animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        statesManager.animationManager.SetBoolForAnimation("isRunning", false);
    }

    public override void HandleState()
    {
        if (patrolDelayTimer < patrolDelay)
        {
            patrolDelayTimer += Time.deltaTime;
        }

        else if (patrolDelayTimer >= patrolDelay)
        {

            if (Vector2.Distance(transform.position, nextPatrollPosition) >= 0.25f)
            {
                statesManager.enemyMovement.MoveTo(transform.position, nextPatrollPosition, patrolSpeed);
            }

            else
            {
                OnPatrolPointReached();
            }
            
        }
      
    }

    private void SetNextPatrolPoint()
    {
        //int patrolDirection = GetPatrolPointDirection();
        int randomRange = Random.Range(4, 7);
        nextPatrollPosition = new Vector2(startPosition.x + (patrolDirection * randomRange), startPosition.y);
        
    }

    private void OnPatrolPointReached()
    {
        SetNextPatrolPoint();
        RandomizePatrolDelay();
        patrolDelayTimer = 0;
    }
    public void SetPatrolDirection(int dir)
    {
        patrolDirection = dir;  
    }
    private int GetPatrolPointDirection()
    {
        int direction = 0;
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        return direction;

    }

    private void RandomizePatrolDelay()
    {
        patrolDelay = Random.Range(2, 5);
    }

}
