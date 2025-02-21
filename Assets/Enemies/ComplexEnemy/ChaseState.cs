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
        if (enemyController == null || enemyController.Player == null || enemyController.PlayerController.IsDead || enemyController.IsDead) return;

        if(enemyController.AttackState.NextAttack)
        {
            if(!enemyController.AttackState.IsEnemyInAttackRange())
            {
                enemyController.EnemyMovement.MoveTo(enemyController.transform.position, enemyController.playerPos, ChaseSpeed);
            }
            else
            {
                enemyController.ChangeState(EnemyStateEnum.Attack);
            }
        }
        else enemyController.ChangeState(EnemyStateEnum.Idle);
       
    }


}
