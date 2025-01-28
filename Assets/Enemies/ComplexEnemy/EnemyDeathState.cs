using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
    [SerializeField] Sprite deadSprite;
    [SerializeField] GameObject atonement;
    public EnemyDeathState() : base()
    {
        stateEnum = EnemyStateEnum.Death;

    }

  

    public override void OnEnterState()
    {
        
        OnEnemyDeath();
        enemyController.EnemyAnimationManager.SetTriggerForAnimation("Death");
        Instantiate(atonement, transform.position, Quaternion.identity);

    }

    public override void OnExitState()
    {

    }

    public override void HandleState()
    {
        
    }

    private void OnEnemyDeath()
    {
        enemyController.IsDead = true;
        enemyController.collisionManager.Rb.bodyType = RigidbodyType2D.Static;
        enemyController.collisionManager.BoxCollider.enabled = false;
        
    }

    public void OnDeathAnimationEnd()
    {
       
        enemyController.EnemyAnimationManager.Animator.enabled = false; 
        enemyController.SpriteRenderer.sprite = deadSprite;
        Destroy(transform.parent.parent.gameObject, 4);
    }

}
