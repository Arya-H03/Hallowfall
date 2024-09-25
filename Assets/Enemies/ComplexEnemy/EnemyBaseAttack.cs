using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAttack : MonoBehaviour
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip [] attackSFX;

    protected EnemyController enemyController;

    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    protected int AttackDamage { get => attackDamage; set => attackDamage = value; }

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();  
    }
    private void Start()
    {
        if (!audioSource)
        {
            Debug.LogWarning(this.name + "has no audio source attached to it's serializefield");
        }
    }
    public virtual void HandleAttack()
    {

    }
}
