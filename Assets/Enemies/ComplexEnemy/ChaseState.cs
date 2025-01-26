using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyBaseState
{
    private float chaseSpeed;
    [SerializeField] float chaseSpeedLrange = 1f;
    [SerializeField] float chaseSpeedUrange = 2f;
    private AudioSource audioSource;
    [SerializeField] AudioClip chaseGroundSFX;
    [SerializeField] AudioClip chaseGrassSFX;
    [SerializeField] AudioClip chaseWoodSFX;

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
        chaseSpeed = Random.Range(chaseSpeedLrange, chaseSpeedUrange + 0.1f);
    }
    public override void OnEnterState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
        enemyController.EnemyMovement.StartRunningSFX(audioSource,chaseGroundSFX,chaseGrassSFX,chaseWoodSFX);
    }

    public override void OnExitState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyMovement.StopRunningSFX(audioSource);
    }

    public override void HandleState()
    {
        // Check if the enemy has seen the player
        if (enemyController == null || enemyController.player == null) return;

        if (enemyController.hasSeenPlayer)
        {
            // Check if the enemy is not facing a ledge
            if (!enemyController.IsFacingLedge || IsFacingPlayerDirection())
            {
                // Check if the player is within attack range
                if (IsPlayerInRange())
                {
                    AttackPlayer();
                }
                else
                {
                    ChaseOrIdle();
                }
            }
            else
            {
                // Enemy is facing a ledge and not aligned with the player
                enemyController.ChangeState(EnemyStateEnum.Idle);
            }
        }
    }

    private bool IsPlayerInRange()
    {
        return Vector2.Distance(enemyController.player.transform.position, transform.position)
               < enemyController.AttackState.AttackRef.AttackRange;
    }

    private bool IsFacingPlayerDirection()
    {
        return enemyController.IsFacingLedge && enemyController.EnemyMovement.FindDirectionToPlayer() == enemyController.transform.localScale.x;
    }

    private void AttackPlayer()
    {
        enemyController.canAttack = true;
        enemyController.ChangeState(EnemyStateEnum.Attack);
        
        
    }

    private void ChaseOrIdle()
    {
        enemyController.canAttack = false;

        PlayerController playerController = enemyController.player.GetComponent<PlayerController>();
        if (!playerController || playerController.IsHanging || playerController.IsDead)
        {
            // If the player is hanging, go to idle state
            enemyController.ChangeState(EnemyStateEnum.Idle);
        }
        else
        {
            enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
            // Otherwise, chase the player
            enemyController.EnemyMovement.MoveTo(transform.position, enemyController.player.transform.position, chaseSpeed);
        }
    }

}
