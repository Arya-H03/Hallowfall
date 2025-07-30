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
        circleCollider = GetComponent<CircleCollider2D>();
        this.stateMachine = enemyController.EnemyStateMachine;

        signalHub.OnEnemyDeath += DisablePhysicsAndCollision;
        signalHub.OnEnemyDeSpawn += EnablePhysicsAndCollision;
    }

    private void OnDisable()
    {
        signalHub.OnEnemyDeath -= DisablePhysicsAndCollision;
        signalHub.OnEnemyDeSpawn -= EnablePhysicsAndCollision;
    }
    public void KnockBackEnemy(Vector2 lanunchVector, float lunchForce)
    {
        StartCoroutine(KnockBackEnemyCoroutine(lanunchVector, lunchForce));
    }
    private IEnumerator KnockBackEnemyCoroutine(Vector2 lanunchVector, float force)
    {
        enemyController.CanMove = false;
        enemyController.IsBeingknocked = true;
        stateMachine.StunState.StunDuration = 1f;
        stateMachine.ChangeState(EnemyStateEnum.Stun);
        Rb.linearVelocity += lanunchVector * luanchModifier * force;
            
        yield return new WaitForSeconds(0.25f);
        enemyController.CanMove = true;
        enemyController.IsBeingknocked = false;
        Rb.linearVelocity -= lanunchVector * luanchModifier * force;
        

    }

    public void OnEnemyParried(GameObject shield, Vector2 hitLocation, int damage)
    {
        PlayerParryState parryState = shield.GetComponentInParent<PlayerParryState>();
        parryState.SpawnImpactEffect(hitLocation);

        if (parryState.CanCounter())
        {
            parryState.CallOnParrySuccessfulEvent();
            enemyController.EnemyHitHandler.HitEnemy(damage, HitSfxType.sword,2);
            Vector3 scale = transform.localScale;
         
        }

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
