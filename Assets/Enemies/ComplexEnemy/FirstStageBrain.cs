//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.MLAgents;
//using Unity.MLAgents.Actuators;
//using System.Runtime.CompilerServices;
//using Unity.MLAgents.Sensors;

//public class FirstStageBrain : Agent
//{
//    [SerializeField] GameObject player;
//    [SerializeField] Transform[] spawnPos;
//    [SerializeField] Transform[] PspawnPos;

//    private EnemyController statesManager;
//    private PatrolState patrolState;
//    private EnemyAttackState attackState;
//    private PlayerController playerController;

//    private void Awake()
//    {
//        statesManager = GetComponent<EnemyController>();

//        patrolState = statesManager.PatrolState;
//        attackState = statesManager.AttackState;

//        playerController = player.GetComponent<PlayerController>();

//    }

//    private void Start()
//    {

//    }

//    public override void OnEpisodeBegin()
//    {
//        //player.transform.position = spawnPos[Random.Range(0, 4)].position;
//        //this.transform.position = PspawnPos[Random.Range(0,3)].position;
//        statesManager.hasSeenPlayer = false;
//        statesManager.canAttack = false;
//        statesManager.isStuned = false;
//        statesManager.ChangeState(EnemyStateEnum.Patrol);

//    }
//    public override void CollectObservations(VectorSensor sensor)
//    {
//        sensor.AddObservation(statesManager.hasSeenPlayer);
//        sensor.AddObservation(player.transform.position);
//        sensor.AddObservation(statesManager.canAttack);
//        sensor.AddObservation(player.GetComponent<Player>().currentHealth);
//        sensor.AddObservation(player.GetComponent<Player>().numberOfHealthShield);

//    }
//    public override void OnActionReceived(ActionBuffers actions)
//    {
//        int stateAction = actions.DiscreteActions[0];
//        int patrolDirAction = actions.DiscreteActions[1];
//        int cancelSwordAttack = actions.DiscreteActions[2];
//        if (!statesManager.isStuned)
//        {
//            switch (stateAction)
//            {

//                //Take no action
//                case 0:
//                    AddReward(-1f / MaxStep);
//                    break;
//                //case 0:
//                //    AddReward(-1f / MaxStep);
//                //    enemyController.ChangeState(EnemyStateEnum.Idle);
//                //    break;

//                //Patrol state acions
//                case 1:
//                    if (!statesManager.hasSeenPlayer)
//                    {
//                        switch (patrolDirAction)
//                        {
//                            //case 0:
//                            //    patrolState.SetPatrolDirection(1);
//                            //    break;
//                            //case 1:
//                            //    patrolState.SetPatrolDirection(-1);
//                            //    break;
//                        }
//                        statesManager.ChangeState(EnemyStateEnum.Patrol);
//                    }

//                    break;
//                //Chase state acions
//                case 2:
//                    if (statesManager.hasSeenPlayer && !statesManager.canAttack)
//                    {
//                        statesManager.ChangeState(EnemyStateEnum.Chase);
//                    }

//                    break;
//                //Attack state acions
//                case 3:
//                    if (statesManager.canAttack)
//                    {
//                        statesManager.ChangeState(EnemyStateEnum.Attack);
//                    }

//                    break;
//            }

//            AddReward(-1f / MaxStep);
//        }



//    }

//    public override void Heuristic(in ActionBuffers actionsOut)
//    {

//        ActionSegment<int> da = actionsOut.DiscreteActions;


//        if (!statesManager.hasSeenPlayer)
//        {
//            da[0] = 1;

//            da[1] = Random.Range(-1, 2);
//        }
//        if (statesManager.hasSeenPlayer)
//        {
//            da[0] = 2;
//        }

//        if (statesManager.canAttack)
//        {
//            da[0] = 3;
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision != null)
//        {

//            if (collision.CompareTag("Mist"))
//            {
//                SetReward(-1f);
//                EndEpisode();
//            }

//        }

//    }
//}
