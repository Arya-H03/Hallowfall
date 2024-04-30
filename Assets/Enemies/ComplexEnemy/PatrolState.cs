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

    private EnemyMovement enemyMovement;
   

    public PatrolState() : base()
    {
        stateEnum = EnemyStateEnum.Patrol;
    }
    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        startPosition = this.transform.position;
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
        if (Vector2.Distance(transform.position, nextPatrollPosition) < 0.25f)
        {
            statesManager.ChangeState(EnemyStateEnum.Idle);
        }
        else
        {
            enemyMovement.MoveTo(transform.position, nextPatrollPosition, patrolSpeed);
        }
    }

    private void SetNextPatrolPoint()
    {
        int patrolDirection = GetPatrolPointDirection();
        int randomRange = Random.Range(4, 7);
        nextPatrollPosition = new Vector2(startPosition.x + (patrolDirection * randomRange), startPosition.y);
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


}
