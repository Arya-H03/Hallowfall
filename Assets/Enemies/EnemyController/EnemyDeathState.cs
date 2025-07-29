using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private EnemyPhysicsHandler collisionManager;
    private EnemyAnimationHandler animationManager;
    private EnemyItemDropHandler itemDropHandler;

    public delegate void EventHandler();
    public EventHandler EnemyBeginDeathEvent;
    public EventHandler EnemyEndDeathEvent;
    public EnemyDeathState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;
        this.itemDropHandler = enemyController.EnemyItemDropHandler;
        this.animationManager = enemyController.EnemyAnimationHandler;
        this.collisionManager = enemyController.EnemyPhysicsHandler;
        EnemyBeginDeathEvent += OnEnemyDeathBegin;
       
    }
    public override void EnterState()
    {
        EnemyBeginDeathEvent?.Invoke();
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    private void OnEnemyDeathBegin()
    {
        enemyController.IsDead = true;
        collisionManager.Rb.bodyType = RigidbodyType2D.Static;
        collisionManager.BoxCollider.enabled = false;
        animationManager.SetTriggerForAnimation("Death");
        enemyController.EnemyHealthbarHandler.DeactiveateHealthbar();

        itemDropHandler.HandleItemDrop(enemyController.transform.position);
    }

    private IEnumerator DeathAnimationEndCoroutine()
    {
        animationManager.Animator.enabled = false;
        enemyController.SpriteRenderer.sprite = enemyConfig.corpseSprite;
        yield return new WaitForSeconds(enemyConfig.corpseLifeTime);
        EnemyEndDeathEvent?.Invoke();

        enemyController.ResetEnemy();

    }

    public void CallDeathAnimationEndCoroutine()
    {
        enemyController.CoroutineRunner.RunCoroutine(DeathAnimationEndCoroutine());
    }

}
