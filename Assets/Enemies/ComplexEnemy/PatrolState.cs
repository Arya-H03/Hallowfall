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

    private float patrolDelayCooldown = 0f;
    private float patrolDelayTimer = 0f;
    private int patrolDirection = 1;

    [SerializeField] private string[] patrolDialogues = {
        " More... More...",
        " It hurts",
        " Help me...",
        " Please make it ... end",
    };

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
        patrolDelayTimer = patrolDelayCooldown;
        enemyController.animationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
    }

    public override void HandleState()
    {

        if (patrolDelayTimer >= patrolDelayCooldown)
        {

            if (Vector2.Distance(transform.position, nextPatrollPosition) >= 0.5f)
            {
                enemyController.EnemyMovement.MoveTo(transform.position, nextPatrollPosition, patrolSpeed);
            }

            else
            {
                OnPatrolPointReached();
            }
            
        }
      
    }

    public void ManagePatrolDelayCooldown()
    {
        if (patrolDelayTimer < patrolDelayCooldown)
        {
            patrolDelayTimer += Time.deltaTime;
        }
        
    }

    private void SetNextPatrolPoint()
    {
        int patrolDirection = GetPatrolPointDirection();
        int randomRange = Random.Range(4, 7);
        nextPatrollPosition = new Vector2(startPosition.x + (patrolDirection * randomRange), startPosition.y);
        enemyController.GetDialogueBox().PlayRandomDialogue(patrolDialogues);
        
    }

    private void OnPatrolPointReached()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
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
       
            patrolDelayCooldown = Random.Range(2, 5);
              
    }

}
