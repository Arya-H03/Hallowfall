using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System.Runtime.CompilerServices;
using Unity.MLAgents.Sensors;

public class SmartEnemyAgent : Agent
{
    [SerializeField] GameObject player;
    [SerializeField] Transform [] spawnPos;
    [SerializeField] Transform [] PspawnPos;

    private EnemyController enemyController;
    private PatrolState patrolState;
    private AttackState attackState;
    private PlayerController playerController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();

        patrolState = enemyController.GetState(EnemyStateEnum.Patrol).GetComponent<PatrolState>();
        attackState = enemyController.GetState(EnemyStateEnum.Attack).GetComponent<AttackState>();

        playerController = player.GetComponent<PlayerController>();

    }

    private void Start()
    {
        
    }

    public override void OnEpisodeBegin()
    {
        player.transform.position = spawnPos[Random.Range(0, 4)].position;
        this.transform.position = PspawnPos[Random.Range(0, 3)].position;
        enemyController.hasSeenPlayer = false;
        enemyController.canAttack = false;
        enemyController.isStuned = false;
        enemyController.ChangeState(EnemyStateEnum.Patrol);
      
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(enemyController.hasSeenPlayer);
        sensor.AddObservation(player.transform.position);
        sensor.AddObservation(enemyController.canAttack);
        sensor.AddObservation(player.GetComponent<Player>().currentHealth);
        sensor.AddObservation(player.GetComponent<Player>().numberOfHealthShield);
        sensor.AddObservation(playerController.isParrying);

 
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int stateAction = actions.DiscreteActions[0];
        int patrolDirAction = actions.DiscreteActions[1];
        int cancelSwordAttack = actions.DiscreteActions[2];
        if (!enemyController.isStuned)
        {
            switch (stateAction)
            {

                //Take no action
                case 0:
                    AddReward(-1f / MaxStep);
                    break;
                //case 0:
                //    AddReward(-1f / MaxStep);
                //    enemyController.ChangeState(EnemyStateEnum.Idle);
                //    break;

                //Patrol state acions
                case 1:
                    if (!enemyController.hasSeenPlayer)
                    {
                        switch (patrolDirAction)
                        {
                            case 0:
                                patrolState.SetPatrolDirection(1);
                                break;
                            case 1:
                                patrolState.SetPatrolDirection(-1);
                                break;
                        }
                        enemyController.ChangeState(EnemyStateEnum.Patrol);
                    }

                    break;
                //Chase state acions
                case 2:
                    if (enemyController.hasSeenPlayer && !enemyController.canAttack)
                    {
                        enemyController.ChangeState(EnemyStateEnum.Chase);
                    }

                    break;
                //Attack state acions
                case 3:
                    if (enemyController.canAttack)
                    {
                        enemyController.ChangeState(EnemyStateEnum.Attack);
                    }

                    if (cancelSwordAttack == 1 && playerController.isParrying)
                    {
                        Debug.Log("cancel");
                        attackState.CancelSwordAttack();
                        AddReward(1f);
                    }
                    break;
            }

            AddReward(-1f / MaxStep);
        }
            


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<int> da = actionsOut.DiscreteActions;


        if (!enemyController.hasSeenPlayer)
        {
            da[0] = 1;

            da[1] = Random.Range(-1, 2);
        }
        if (enemyController.hasSeenPlayer && !enemyController.canAttack)
        {
            da[0] = 2;
        }

        if (enemyController.canAttack)
        {
            da[0] = 3;

            if (playerController.isParrying)
            {
                da[2] = 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null )
        {
     
            //if (collision.CompareTag("Mist"))
            //{
            //    SetReward(-1f);
            //    EndEpisode();
            //}

        }
        
    }
}
