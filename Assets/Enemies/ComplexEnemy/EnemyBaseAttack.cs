using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAttack : MonoBehaviour
{
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    private bool isAvailable = true;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] attackSFX;

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

    public void StartAttack()
    {
        StartCoroutine(AttackCoroutine());
    }
    private IEnumerator AttackCoroutine()
    {
        HandleAttack();
        isAvailable = false;
        yield return new WaitForSeconds(attackCooldown);
        isAvailable = true;
    }
    protected virtual void HandleAttack()
    {

    }

    protected void PlayAttackSFX()
    {
        if (audioSource != null && attackSFX.Length > 0)
        {
            AudioManager.Instance.PlayRandomSFX(audioSource, attackSFX, 1);
        }
    }
}
