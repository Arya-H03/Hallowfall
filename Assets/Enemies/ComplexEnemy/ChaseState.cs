using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseState : EnemyBaseState
{
    [SerializeField] LayerMask playerLayer;
    private float visionAngle = 10f;
    private float visionRange = 3f;
    [SerializeField] Transform sightCenter;

    private bool isPlayerInSight = false;

    private float chaseSpeed;
    [SerializeField] float minChaseSpeed = 1f;
    [SerializeField] float maxChaseSpeed = 2f;


    private AudioSource audioSource;
    [SerializeField] AudioClip chaseGroundSFX;
    [SerializeField] AudioClip chaseGrassSFX;
    [SerializeField] AudioClip chaseWoodSFX;

    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }

    public ChaseState() : base()
    {
        stateEnum = EnemyStateEnum.Chase;    
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChaseSpeed = Random.Range(minChaseSpeed, maxChaseSpeed + 0.1f); 
    }
    public override void OnEnterState()
    {
        enemyController.NavAgent.isStopped = false;
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        enemyController.NavAgent.isStopped = true;
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyMovement.StopRunningSFX(audioSource);
    }

    public override void HandleState()
    {
        if (enemyController == null || enemyController.Player == null || enemyController.PlayerController.IsDead || enemyController.IsDead || enemyController.AttackState.NextAttack == null)
        {
            enemyController.ChangeState(EnemyStateEnum.Idle);
            return;
        }

        CheckEnemySight();

        //if (enemyController.AttackState.NextAttack == null)
        //{
        //    enemyController.ChangeState(EnemyStateEnum.Idle);
        //    return;
        //}

        bool isInRange = enemyController.AttackState.IsEnemyInAttackRange();
        EnemyAttackTypeEnum attackType = enemyController.AttackState.NextAttack.AttackTypeEnum;

        if (!isInRange)
        {
            enemyController.EnemyMovement.MoveToPlayer(ChaseSpeed);
        }
            
        else
        {
            enemyController.ChangeState(EnemyStateEnum.Attack);
        }
           

        //switch (attackType)
        //{
        //    case EnemyAttackTypeEnum.Melee:
        //        if ((!isInRange || !isPlayerInSight))
        //            enemyController.EnemyMovement.MoveToPlayer(ChaseSpeed);
        //        else
        //            enemyController.ChangeState(EnemyStateEnum.Attack);
        //        break;

        //    case EnemyAttackTypeEnum.Ranged:
        //        if (!isInRange)
        //            enemyController.EnemyMovement.MoveToPlayer(ChaseSpeed);
        //        else
        //            enemyController.ChangeState(EnemyStateEnum.Attack);
        //        break;
        //}





    }

    private void CheckEnemySight()
    {
        Vector2 startDirection =new Vector3(-enemyController.transform.localScale.x, 0, 0);

        RaycastHit2D hit1 = Physics2D.Raycast(sightCenter.position, Quaternion.Euler(0f, 0f, -visionAngle / 2f) * startDirection, visionRange, playerLayer);
        Debug.DrawRay(sightCenter.position, (Quaternion.Euler(0f, 0f, -visionAngle / 2f) * startDirection) * visionRange, Color.red);

        RaycastHit2D hit2 = Physics2D.Raycast(sightCenter.position, Quaternion.Euler(0f, 0f, visionAngle / 2f) * startDirection, visionRange, playerLayer);
        Debug.DrawRay(sightCenter.position, (Quaternion.Euler(0f, 0f, visionAngle / 2f) * startDirection) * visionRange, Color.red);

        if(hit1 &&  hit2) isPlayerInSight = true;
        else isPlayerInSight =  false;


    }


}
