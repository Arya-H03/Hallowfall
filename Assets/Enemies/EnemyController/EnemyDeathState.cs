using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
   
    public delegate void EventHandler();
    public EventHandler EnemyBeginDeathEvent;
    public EventHandler EnemyEndDeathEvent;
    public EnemyDeathState(EnemyController enemyController, EnemyStateEnum stateEnum,EnemyConfigSO enemyConfig) : base(enemyController, stateEnum, enemyConfig)
    {
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
        enemyController.CollisionManager.Rb.bodyType = RigidbodyType2D.Static;
        enemyController.CollisionManager.BoxCollider.enabled = false;
        enemyController.EnemyAnimationManager.SetTriggerForAnimation("Death");
        enemyController.WorldCanvas.gameObject.SetActive(false);

        enemyController.ItemDropHandler.HandleItemDrop(enemyController.transform.position);
    }

    private IEnumerator DeathAnimationEndCoroutine()
    {
        enemyController.EnemyAnimationManager.Animator.enabled = false;
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
