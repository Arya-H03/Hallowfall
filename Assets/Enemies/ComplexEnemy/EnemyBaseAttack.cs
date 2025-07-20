using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAttack : MonoBehaviour
{
    [SerializeField] protected string animCondition;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int attackDamage;
    protected bool isAvailable = true;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] attackSFX;
    [SerializeField] protected EnemyAttackTypeEnum attackTypeEnum;
    [SerializeField] protected GameObject attackZoneGO;


    protected EnemyController enemyController;

    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    protected int AttackDamage { get => attackDamage; set => attackDamage = value; }
    public string AnimCondition { get => animCondition; set => animCondition = value; }
    public EnemyAttackTypeEnum AttackTypeEnum { get => attackTypeEnum;}


    protected virtual void Awake()
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
    public virtual IEnumerator AttackCoroutine()
    {
        isAvailable = false;
        enemyController.AttackState.RemoveFromAvailableAttacks(this);
        enemyController.EnemyAnimationManager.SetBoolForAnimation(AnimCondition, true);
       
        yield return new WaitForSeconds(attackCooldown);
        isAvailable = true;
        enemyController.AttackState.AddToAvailableAttacks(this);
    }
    public virtual void CallAttackActionOnAnimFrame()
    {
        
    }

    protected void PlayAttackSFX()
    {
        if (audioSource != null && attackSFX.Length > 0)
        {
            AudioManager.Instance.PlaySFX(attackSFX,enemyController.transform.position,1);
        }
    }

    public virtual void OnAttackEnd()
    {
        DeactivateZoneAttack();
    }

    public void DeactivateZoneAttack()
    {
        if (attackZoneGO)
        {
            attackZoneGO.SetActive(false);
        }

    }
}
