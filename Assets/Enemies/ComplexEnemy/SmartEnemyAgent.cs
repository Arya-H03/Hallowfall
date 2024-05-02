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

    private EnemyStatesManager statesManager;
    private PatrolState patrolState;

    private void Awake()
    {
        statesManager = GetComponent<EnemyStatesManager>();
        
    }

    private void Start()
    {
        patrolState = gameObject.GetComponent<PatrolState>();
    }

    public override void OnEpisodeBegin()
    {
        player.transform.position = spawnPos[Random.Range(0, 4)].position;
        this.transform.position = PspawnPos[Random.Range(0,3)].position;
        statesManager.hasSeenPlayer = false;
        statesManager.canAttack = false;
        statesManager.ChangeState(EnemyStateEnum.Idle);
      
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(statesManager.hasSeenPlayer);
        sensor.AddObservation(player.transform.position);
        sensor.AddObservation(statesManager.canAttack);
        sensor.AddObservation(player.GetComponent<Player>().currentHealth);
        sensor.AddObservation(player.GetComponent<Player>().numberOfHealthShield);

 
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int stateAction = actions.DiscreteActions[0];
        

        switch (stateAction)
        {
            case 0:
                AddReward(-1f / MaxStep);
                break;
            //case 0:
            //    AddReward(-1f / MaxStep);
            //    statesManager.ChangeState(EnemyStateEnum.Idle);
            //    break;
            case 1:
                if (!statesManager.hasSeenPlayer)
                {
                    switch (actions.DiscreteActions[1])
                    {
                        case 0:
                            patrolState.SetPatrolDirection(1);
                            break;
                        case 1:
                            patrolState.SetPatrolDirection(-1);
                            break;
                    }
                    statesManager.ChangeState(EnemyStateEnum.Patrol);
                }
                
                break;
            case 2:
                if (statesManager.hasSeenPlayer && !statesManager.canAttack)
                {
                    statesManager.ChangeState(EnemyStateEnum.Chase);
                }
                
                break;
            case 3:
                if(statesManager.canAttack)
                {
                    statesManager.ChangeState(EnemyStateEnum.Attack);
                }
                break;
        }

        AddReward(-1f/MaxStep);


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<int> da = actionsOut.DiscreteActions;


        if (!statesManager.hasSeenPlayer)
        {
            da[0] = 1;

            da[1] = Random.Range(-1, 2);
        }
        if (statesManager.hasSeenPlayer)
        {
            da[0] = 2;
        }

        if (statesManager.canAttack)
        {
            da[0] = 3;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null )
        {
     
            if (collision.CompareTag("Mist"))
            {
                SetReward(-1f);
                EndEpisode();
            }

        }
        
    }
}
