using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    private int damage;
    private Vector2 hitPoint;
    [SerializeField] private float hitDelay = 0.5f;

    public int Damage { get => damage; set => damage = value; }
    public Vector2 HitPoint { get => hitPoint; set => hitPoint = value; }

    public EnemyHitState() : base()
    {
        stateEnum = EnemyStateEnum.Hit;

    }

    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }

    public override void OnEnterState()
    {
        StartCoroutine(EnemyHitCoroutine(damage, hitPoint, hitDelay));
    }

    public override void OnExitState()
    {
        
    }

    public override void HandleState()
    {


    }

    private IEnumerator EnemyHitCoroutine(int damage, Vector2 hitPoint, float delay)
    {
        //VFX
        PlayBloodEffect(hitPoint);
        enemyController.Material.SetFloat("_Flash", 1);
        //SFX

        //Damage
        enemyController.OnEnemyTakingDamage(damage, enemyController.DamageModifier);

        //Wait?
        yield return new WaitForSeconds(delay);
        enemyController.Material.SetFloat("_Flash", 0);
        enemyController.ChangeState(EnemyStateEnum.Idle);

    }

    private void PlayBloodEffect(Vector2 hitPoint)
    {
        Vector2 distance = hitPoint - new Vector2(this.transform.position.x, this.transform.position.y);
        var shape = enemyController.BloodParticles.shape;
        shape.position = (distance);
        enemyController.BloodParticles.Play();
    }

}
