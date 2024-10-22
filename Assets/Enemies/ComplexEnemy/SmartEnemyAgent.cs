//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.MLAgents;
//using Unity.MLAgents.Actuators;
//using System.Runtime.CompilerServices;
//using Unity.MLAgents.Sensors;
//using static EnemyAttackState;

//public class SmartEnemyAgent : Agent
//{
//    //[SerializeField] GameObject target;
//    //[SerializeField] Transform[] spawnPos;
//    //[SerializeField] Transform[] PspawnPos;

//    //private EnemyController enemyController;
//    //private PatrolState patrolState;
//    //private SwordAttackState swordAttackState;
//    //private TAD tad;
//    //private PlayerController playerController;

//    //private void Awake()
//    //{
//    //    enemyController = GetComponent<EnemyController>();

//    //    patrolState = enemyController.GetState(EnemyStateEnum.Patrol).GetComponent<PatrolState>();
//    //    swordAttackState = enemyController.GetState(EnemyStateEnum.Combat).GetComponent<SwordAttackState>();

//    //    playerController = target.GetComponent<PlayerController>();
//    //    tad = target.GetComponent<TAD>();

//    //}

//    //private void Start()
//    //{

//    //}

//    //public override void OnEpisodeBegin()
//    //{
//    //    target.transform.position = spawnPos[Random.Range(0, 4)].position;
//    //    this.transform.position = PspawnPos[Random.Range(0, 3)].position;
//    //    enemyController.hasSeenPlayer = false;
//    //    enemyController.canAttack = false;
//    //    enemyController.isStuned = false;
//    //    swordAttackState.SetIsAttacking(false);
//    //    enemyController.ChangeState(EnemyStateEnum.Patrol);
//    //    enemyController.ResetHealth();
//    //    tad.ResetHealth();

//    //}
//    //public override void CollectObservations(VectorSensor sensor)
//    //{
//    //    sensor.AddObservation(enemyController.hasSeenPlayer);
//    //    sensor.AddObservation(target.transform.position);
//    //    sensor.AddObservation(enemyController.canAttack);
//    //    sensor.AddObservation(enemyController.GetCanBlock());
//    //    sensor.AddObservation(enemyController.GetComponent<BlockState>().GetBlockTimer());
//    //    sensor.AddObservation(enemyController.currentHealth);
//    //    sensor.AddObservation(tad.currentHealth);
//    //    sensor.AddObservation(tad.isAttaking);

//    //    sensor.AddObservation(target.GetComponent<Player>().currentHealth);
//    //    sensor.AddObservation(target.GetComponent<Player>().numberOfHealthShield);
//    //    sensor.AddObservation(playerController.isParrying);


//    //}
//    //public override void OnActionReceived(ActionBuffers actions)
//    //{
//    //    int stateAction = actions.DiscreteActions[0];
//    //    if (!enemyController.isStuned && enemyController.currentStateEnum == EnemyStateEnum.Combat)
//    //    {
//    //        if (stateAction == 2 || stateAction == 3)
//    //        {
//    //            Debug.Log(stateAction);
//    //        }

//    //        switch (stateAction)
//    //        {
//    //            Take no action
//    //            case 0:
//    //            AddReward(-1f / MaxStep);
//    //            swordAttackState.ChangeAction(CombatActionEnum.None);
//    //            break;
//    //            Move action
//    //            case 1:
//    //            if (!swordAttackState.GetIsInAttackRange())
//    //            {
//    //                swordAttackState.ChangeAction(CombatActionEnum.Move);
//    //            }

//    //            break;
//    //            Sword Attack state acions
//    //            case 2:
//    //            if (swordAttackState.GetIsInAttackRange() && swordAttackState.GetCanSwordAttack())
//    //            {
//    //                swordAttackState.ChangeAction(CombatActionEnum.SwordAttack);
//    //            }

//    //            break;
//    //        case 3:
//    //            if (/*enemyController.player.GetComponent<PlayerController>().isAttacking*/ tad.isAttaking && enemyController.GetCanBlock())
//    //            {
//    //                swordAttackState.ChangeAction(CombatActionEnum.Block);
//    //            }

//    //            break;
//    //        }

//    //        AddReward(-1f / MaxStep);
//    //    }






//    //    int stateAction = actions.DiscreteActions[0];
//    //    int cancelSwordAttack = actions.DiscreteActions[2];
//    //    if (!enemyController.isStuned)
//    //    {
//    //        switch (stateAction)
//    //        {

//    //            //Take no action
//    //            case 0:
//    //                AddReward(-1f / MaxStep);
//    //                break;
//    //            //Chase state acions
//    //            case 1:
//    //                if (enemyController.hasSeenPlayer /*&& !enemyController.canAttack*/)
//    //                {
//    //                    enemyController.ChangeState(EnemyStateEnum.Chase);
//    //                }
//    //                break;
//    //            //Attack state acions
//    //            case 2:
//    //                if (enemyController.canAttack && enemyController.hasSeenPlayer)
//    //                {
//    //                    enemyController.ChangeState(EnemyStateEnum.Attack);
//    //                }

//    //                if (cancelSwordAttack == 1 && playerController.isParrying)
//    //                {
//    //                    Debug.Log("canceled sword attack");
//    //                    swordAttackState.CancelAttack();
//    //                    AddReward(1f);
//    //                }
//    //                break;
//    //            case 3:
//    //                if (playerController.isAttacking && enemyController.hasSeenPlayer)
//    //                {
//    //                    enemyController.ChangeState(EnemyStateEnum.Block);
//    //                }
//    //                break;
//    //        }

//    //        AddReward(-1f / MaxStep);
//    //    }



//    //}

//    //public override void Heuristic(in ActionBuffers actionsOut)
//    //{

//    //    ActionSegment<int> da = actionsOut.DiscreteActions;


//    //    if (enemyController.hasSeenPlayer)
//    //    {
//    //        da[0] = 1;
//    //    }
//    //    if (enemyController.canAttack && enemyController.hasSeenPlayer)
//    //    {
//    //        da[0] = 2;

//    //        if (playerController.isParrying)
//    //        {
//    //            da[2] = 1;
//    //        }
//    //    }

//    //    if (playerController.isAttacking && enemyController.hasSeenPlayer)
//    //    {
//    //        da[0] = 3;


//    //    }
//    //}

//    //private void OnTriggerEnter2D(Collider2D collision)
//    //{
//    //    if (collision != null)
//    //    {

//    //        if (collision.CompareTag("TAD"))
//    //        {
//    //            SetReward(-1f);
//    //            EndEpisode();
//    //        }

//    //    }

//    //}
//}
