using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : EnemyBaseState
{
    private GameObject swordBlockObj;

    [SerializeField]private float blockMeter;
    private float blockMeterMax = 100;
    [SerializeField] private float blockTimer = 0f;
    private float blockTimerCooldown = 10f;

    private float blockDuration = 5f;

    public BlockState() : base()
    {
        stateEnum = EnemyStateEnum.Block;

    }

    private void Awake()
    {
        swordBlockObj = GameObject.FindGameObjectWithTag("EnemySwordBlock");
    }
    private void Start()
    {
        blockMeter = blockMeterMax;
        blockTimer = blockTimerCooldown;
    }

    public override void OnEnterState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isRunning", false);
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isAttackingSword", false);
        enemyController.EnemyAnimationManager.SetTriggerForAnimation("Block");
        
    }

    public override void OnExitState()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isBlocking", false);
        swordBlockObj.GetComponent<BoxCollider2D>().enabled = false;
        Debug.Log("End block");
    }

    public override void HandleState()
    {


    }

    public void BeginBlockingSword()
    {
        enemyController.EnemyAnimationManager.SetBoolForAnimation("isBlocking", true);
        enemyController.SetCanBlock(false);
        blockTimer = 0f;
        swordBlockObj.GetComponent<BoxCollider2D>().enabled = true;
        StartCoroutine(EndBlockByDuration(blockDuration, EnemyStateEnum.Chase));
    }

    public void OnAttackBlocked(float value, Vector2 knockBackVel,GameObject player)
    {
        enemyController.collisionManager.LaunchEnemy(knockBackVel);
        //enemyController.agent.SetReward(1f);
        if(blockMeter > 0)
        {
            blockMeter -= value;
            if(blockMeter <= 0)
            {
                enemyController.collisionManager.LaunchEnemy(knockBackVel * 5);
                player.GetComponent<PlayerCollision>().KnockPlayer(new Vector2(-knockBackVel.x * 5, knockBackVel.y * 5));
                blockMeter = blockMeterMax;
                StartCoroutine(EndBlockByDuration(0, EnemyStateEnum.Chase));
                //enemyController.ChangeState(enemyController.previousStateEnum);
            }
        }
    }

    public void ManageBlockCooldown()
    {
        if (blockTimer < blockTimerCooldown)
        {
            blockTimer += Time.deltaTime;
            if(blockTimer >= blockTimerCooldown) 
            {
                enemyController.SetCanBlock(true);
            }
        }
    }

    public float GetBlockTimer()
    {
        return blockTimer;
    }

    private IEnumerator EndBlockByDuration(float duration,EnemyStateEnum stateToGoBack)
    {
        if(enemyController.CurrentStateEnum == EnemyStateEnum.Block)
        {
            yield return new WaitForSeconds(duration);
            enemyController.ChangeState(stateToGoBack);
        }
       
    }


}
