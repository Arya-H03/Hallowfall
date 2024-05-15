using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockState : EnemyBaseState
{
    private GameObject swordBlockObj;

    [SerializeField]private float blockMeter;
    private float blockMeterMax = 100;
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
    }

    public override void OnEnterState()
    {
        enemyController.animationManager.SetBoolForAnimation("isRunning", false);
        enemyController.animationManager.SetBoolForAnimation("isAttackingSword", false);

        enemyController.animationManager.SetTriggerForAnimation("Block");
        
        
    }

    public override void OnExitState()
    {
        enemyController.animationManager.SetBoolForAnimation("isBlocking", false);
        swordBlockObj.GetComponent<BoxCollider2D>().enabled = false;
    }

    public override void HandleState()
    {


    }

    public void BeginBlockingSword()
    {
        enemyController.animationManager.SetBoolForAnimation("isBlocking", true);
        swordBlockObj.GetComponent<BoxCollider2D>().enabled = true; 
    }

    public void OnAttackBlocked(float value, Vector2 knockBackVel,GameObject player)
    {
        enemyController.collisionManager.LaunchEnemy(knockBackVel);
        if(blockMeter > 0)
        {
            blockMeter -= value;
            if(blockMeter <= 0)
            {
                enemyController.collisionManager.LaunchEnemy(knockBackVel * 5);
                player.GetComponent<PlayerCollision>().KnockPlayer(new Vector2(-knockBackVel.x * 5, knockBackVel.y * 5));
                blockMeter = blockMeterMax;
                enemyController.ChangeState(enemyController.previousStateEnum);
            }
        }
    }


}
