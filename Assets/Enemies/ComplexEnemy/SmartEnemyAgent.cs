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

    private void Awake()
    {
        int fpsTarget = 60;
        Application.targetFrameRate = fpsTarget;
    }

    public override void OnEpisodeBegin()
    {
        this.transform.position = spawnPos[Random.Range(0,4)].position;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(player);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];

        //Debug.Log(actions.ContinuousActions[0]);

        transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * 3f;


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
                SetReward(1f);
                EndEpisode();
                Debug.Log("Player");
            }

            if (collision.CompareTag("Mist"))
            {
                SetReward(-5f);
                EndEpisode();
            }



        }
        
    }
}
