using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAttack : MonoBehaviour
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;

    protected EnemyController enemyController;

    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    protected int AttackDamage { get => attackDamage; set => attackDamage = value; }

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();  
    }
    public virtual void HandleAttack()
    {

    }
}
