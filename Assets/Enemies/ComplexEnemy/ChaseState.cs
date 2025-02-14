using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseState : EnemyBaseState
{
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
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
    }

    public override void OnExitState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyMovement.StopRunningSFX(audioSource);
    }

    public override void HandleState()
    {
        // Check if the enemy has seen the player
        if (enemyController == null || enemyController.Player == null) return;

        if (! enemyController.PlayerController.IsDead)
        {
            // Check if the enemy is not facing a ledge
            if (!enemyController.IsFacingLedge || IsFacingPlayerDirAtLedge())
            {
                // Check if the player is within attack range
                if (enemyController.AttackState.IsEnemyInAttackRange() && enemyController.canAttack && enemyController.AttackState.IsAttackDelayOver)
                {
                    enemyController.ChangeState(EnemyStateEnum.Attack);
                }
                else if(!enemyController.AttackState.IsEnemyInAttackRange())
                {

                    
                    enemyController.EnemyMovement.MoveTo(transform.position, enemyController.playerPos, ChaseSpeed);
                    //ChaseOrIdle();
                }
                else
                {
                    enemyController.ChangeState(EnemyStateEnum.Idle);
                }
            }
            else
            {
                // Enemy is facing a ledge and not aligned with the player
                enemyController.ChangeState(EnemyStateEnum.Idle);
            }
        }
    }

    private bool IsFacingPlayerDirAtLedge()
    {
        return enemyController.IsFacingLedge && enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x;
    }

    private void AttackPlayer()
    {
        //enemyController.canAttack = true;
        enemyController.ChangeState(EnemyStateEnum.Attack);
        
        
    }

    private void ChaseOrIdle()
    {
        //enemyController.canAttack = false;

        PlayerController playerController = enemyController.Player.GetComponent<PlayerController>();
        if (!playerController || playerController.IsHanging || playerController.IsDead || !enemyController.canAttack ||!enemyController.AttackState.IsAttackDelayOver)
        {
            // If the player is hanging, go to idle state
            enemyController.ChangeState(EnemyStateEnum.Idle);
        }
        else
        {
            //enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
            // Otherwise, chase the player
            enemyController.EnemyMovement.MoveTo(transform.position, enemyController.PlayerPos, ChaseSpeed);
            
        }
    }

}
