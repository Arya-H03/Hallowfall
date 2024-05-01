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
        patrolState = GetComponent<PatrolState>();  
    }

    public override void OnEpisodeBegin()
    {
        player.transform.position = spawnPos[Random.Range(0, 4)].position;
        this.transform.position = PspawnPos[Random.Range(0,3)].position;
        statesManager.hasSeenPlayer = false;
        statesManager.ChangeState(EnemyStateEnum.Idle);
       

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(statesManager.hasSeenPlayer);
 
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        int stateAction = actions.DiscreteActions[0];

        switch (stateAction)
        {
            case 0:
                break;
            case 1:
                AddReward(-1f/MaxStep);
                statesManager.ChangeState(EnemyStateEnum.Idle);
                break;               
            case 2:
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
            case 3:
                if (statesManager.hasSeenPlayer)
                {
                    statesManager.ChangeState(EnemyStateEnum.Chase);
                }
                
                break;
        }

        AddReward(-1f/MaxStep);


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> ca = actionsOut.ContinuousActions;

        ca[0] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null )
        {
            
            if(collision.CompareTag("Player"))
            {
                SetReward(2f);
                EndEpisode();
            }

            if (collision.CompareTag("Mist"))
            {
                SetReward(-1f);
                EndEpisode();
            }



        }
        
    }
}
