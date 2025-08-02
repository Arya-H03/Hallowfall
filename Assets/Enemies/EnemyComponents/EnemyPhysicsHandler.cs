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

        signalHub.OnEnemyDeath += DisablePhysicsAndCollision;
        signalHub.OnEnemyDeSpawn += EnablePhysicsAndCollision;
        signalHub.OnEnemyHit += KnockBackEnemy;
    }

    private void OnDisable()
    {
        if (signalHub == null) return;
        signalHub.OnEnemyDeath -= DisablePhysicsAndCollision;
        signalHub.OnEnemyDeSpawn -= EnablePhysicsAndCollision;
        signalHub.OnEnemyHit -= KnockBackEnemy;
    }
    public void KnockBackEnemy(float f, HitSfxType h)
    {
        if(!enemyController.IsBeingknocked && rb.bodyType != RigidbodyType2D.Static)
        {
            StartCoroutine(KnockBackEnemyCoroutine(1));
        }
       
    }
    private IEnumerator KnockBackEnemyCoroutine(float force)
    {
        Vector2 dir = -(enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos()).normalized;
        enemyController.CanMove = false;
        enemyController.IsBeingknocked = true;
        stateMachine.StunState.StunDuration = 1f;
        Rb.linearVelocity += dir * luanchModifier * force;
            
        yield return new WaitForSeconds(0.25f);
        enemyController.CanMove = true;
        enemyController.IsBeingknocked = false;
        if(rb.bodyType != RigidbodyType2D.Static) Rb.linearVelocity = Vector2.zero;



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
