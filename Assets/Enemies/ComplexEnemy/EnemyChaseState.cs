using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChaseState : EnemyBaseState
{
    [SerializeField] LayerMask playerLayer;
    private float visionAngle = 10f;
    private float visionRange = 3f;
    [SerializeField] Transform sightCenter;

    private float chaseSpeed;
    [SerializeField] float minChaseSpeed = 1f;
    [SerializeField] float maxChaseSpeed = 2f;


    [SerializeField] AudioClip chaseGroundSFX;
    [SerializeField] AudioClip chaseGrassSFX;
    [SerializeField] AudioClip chaseWoodSFX;

    private bool isInPlayerRange = false;
    EnemyAttackTypeEnum attackType;

    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }

    public EnemyChaseState() : base()
    {
        stateEnum = EnemyStateEnum.Chase;    
    }

    private void Start()
    {
        ChaseSpeed = Random.Range(minChaseSpeed, maxChaseSpeed + 0.1f); 
    }
    public override void OnEnterState()
    {
        
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {      
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);       
    }

    public override void UpdateLogic()
    {
        if (enemyController == null || enemyController.Player == null || enemyController.PlayerController.IsDead || enemyController.IsDead || enemyController.AttackState.NextAttack == null || enemyController.EnemyMovementHandler.IsCurrentCellBlockedByEnemies())          
        {
            enemyController.ChangeState(EnemyStateEnum.Idle);
            return;
        }
        isInPlayerRange = enemyController.AttackState.IsEnemyInAttackRange();
        if (isInPlayerRange)
        {
            attackType = enemyController.AttackState.NextAttack.AttackTypeEnum;
            enemyController.ChangeState(EnemyStateEnum.Attack);
        }
    }

    public override void FixedUpdateLogic()
    {
        if (!isInPlayerRange)
        {
            enemyController.EnemyMovementHandler.MoveToPlayer(ChaseSpeed);
        }
    }

  
}
