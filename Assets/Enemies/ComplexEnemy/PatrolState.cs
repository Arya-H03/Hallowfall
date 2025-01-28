using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static EnemyAI;

public class PatrolState : EnemyBaseState
{
    private Vector3 nextPatrollPosition;
    private Vector3 startPosition;
    private float patrolSpeed = 1f;

    private float patrolDelayCooldown = 0f;
    private float patrolDelayTimer = 0f;

    [SerializeField] Transform rightPatrolBound;
    [SerializeField] Transform leftPatrolBound;

    private AudioSource audioSource;
    [SerializeField] AudioClip walkGroundSFX;
    [SerializeField] AudioClip walkGrassSFX;
    [SerializeField] AudioClip walkWoodSFX;

    [SerializeField]
    private string[] patrolDialogues = {
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
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //startPosition = this.transform.position;
        //SetNextPatrolPoint();
    }
    public override void OnEnterState()
    {
        //SetNextPatrolPoint();
        //patrolDelayTimer = patrolDelayCooldown;
        StartCoroutine(PatrolToNextPoint());
        //enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
        

    }

    public override void OnExitState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyMovement.StopRunningSFX(audioSource);
    }

    private IEnumerator PatrolToNextPoint()
    {
        if(enemyController.CurrentStateEnum == EnemyStateEnum.Patrol)
        {
            SetNextPatrolPoint();
            enemyController.EnemyMovement.StartRunningSFX(audioSource, walkGroundSFX, walkGrassSFX, walkWoodSFX);

            while (Vector2.Distance(transform.position, nextPatrollPosition) >= 0.5f)
            {
                if (!enemyController.IsFacingLedge)
                {
                    enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
                    enemyController.EnemyMovement.MoveTo(transform.position, nextPatrollPosition, patrolSpeed);
                    yield return new WaitForEndOfFrame();
                }
                else break;
            }
            OnPatrolPointReached();


            int delay = Random.Range(1, 4);
            yield return new WaitForSeconds(delay);

            StartCoroutine(PatrolToNextPoint());
        }
        
        
    }
    public override void HandleState()
    {

        //if (patrolDelayTimer >= patrolDelayCooldown)
        //{
            //if (!enemyController.IsFacingLedge)
            //{
            //    if (Vector2.Distance(transform.position, nextPatrollPosition) >= 0.5f)
            //    {
                    
            //    }

            //    else
            //    {
            //        OnPatrolPointReached();
            //    }
            //}



        //}

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
        int randomXpos = Random.Range(Mathf.FloorToInt(leftPatrolBound.position.x), Mathf.CeilToInt(rightPatrolBound.position.x));
        nextPatrollPosition = new Vector2(randomXpos, startPosition.y);       
    }

    private void OnPatrolPointReached()
    {
        enemyController.EnemyMovement.StopRunningSFX(audioSource);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        //SetNextPatrolPoint();
        //RandomizePatrolDelay();
        //patrolDelayTimer = 0;

    }
    //public void SetPatrolDirection(int dir)
    //{
    //    patrolDirection = dir;  
    //}
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
