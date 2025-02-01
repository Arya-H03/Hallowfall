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
        ChaseSpeed = Random.Range(chaseSpeedLrange, chaseSpeedUrange + 0.1f);
    }
    public override void OnEnterState()
    {
        //enemyController.EnemyMovement.StartRunningSFX(audioSource, chaseGroundSFX, chaseGrassSFX, chaseWoodSFX);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
        //enemyController.EnemyMovement.StartRunningSFX(audioSource,chaseGroundSFX,chaseGrassSFX,chaseWoodSFX);
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
                    
                    enemyController.EnemyMovement.MoveTo(transform.position, enemyController.player.transform.position, ChaseSpeed);
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

        PlayerController playerController = enemyController.player.GetComponent<PlayerController>();
        if (!playerController || playerController.IsHanging || playerController.IsDead || !enemyController.canAttack ||!enemyController.AttackState.IsAttackDelayOver)
        {
            // If the player is hanging, go to idle state
            enemyController.ChangeState(EnemyStateEnum.Idle);
        }
        else
        {
            //enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", true);
            // Otherwise, chase the player
            enemyController.EnemyMovement.MoveTo(transform.position, enemyController.player.transform.position, ChaseSpeed);
            
        }
    }

}
