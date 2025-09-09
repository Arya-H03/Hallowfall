using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPhysicsHandler : MonoBehaviour, IInitializeable<EnemyController>
{

    private Rigidbody2D rb;

    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;

    private EnemySignalHub signalHub;
    private EnemyController enemyController;
    private EnemyStateMachine stateMachine;

    

    [SerializeField] float luanchModifier = 1f;
    [SerializeField] GameObject impactEffect;
   
    public BoxCollider2D BoxCollider { get => boxCollider; set => boxCollider = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }
  

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        signalHub = enemyController.SignalHub;
        this.Rb = enemyController.Rb;
        this.boxCollider = enemyController.BoxCollider;
        circleCollider = GetComponentInParent<CircleCollider2D>();
        this.stateMachine = enemyController.EnemyStateMachine;

        signalHub.OnDisablePhysicsAndCollision += DisablePhysicsAndCollision;
        signalHub.OnEnablePhysicsAndCollision += EnablePhysicsAndCollision;
        signalHub.OnEnemyKnockBack += KnockBackEnemy;
    }

    //private void OnDisable()
    //{
    //    if (signalHub == null) return;
    //    signalHub.OnDisablePhysicsAndCollision -= DisablePhysicsAndCollision;
    //    signalHub.OnEnablePhysicsAndCollision -= EnablePhysicsAndCollision;
    //    signalHub.OnEnemyKnockBack -= KnockBackEnemy;
    //}
    private void KnockBackEnemy(Vector2 dir,float force)
    {
        StartCoroutine(KnockBackEnemyCoroutine(dir,force));
    }
    private IEnumerator KnockBackEnemyCoroutine(Vector2 dir,float force)
    {
        if (enemyController.IsBeingknocked) yield break;

        enemyController.CanMove = false;
        enemyController.IsBeingknocked = true;

        Rb.linearVelocity += dir * luanchModifier * force;
            
        yield return new WaitForSeconds(0.25f);
        enemyController.CanMove = true;
        enemyController.IsBeingknocked = false;
        Rb.linearVelocity = Vector2.zero;



    }

    public void OnEnemyParried(float damage)
    {
        enemyController.PlayerController.PlayerSignalHub.OnEnemyParried?.Invoke(enemyController, damage);
    }
    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    private void DisablePhysicsAndCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        circleCollider.enabled = false;
        boxCollider.enabled = false;
    }

    private void EnablePhysicsAndCollision()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        circleCollider.enabled = true;
        boxCollider.enabled = true;
    }
}
