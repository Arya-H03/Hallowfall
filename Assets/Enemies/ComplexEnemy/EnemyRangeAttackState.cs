using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttackState : EnemyBaseState
{
    [SerializeField] private EnemyAbilitiesEnum abilitiesEnum;

    [SerializeField] private float rangeAttackCooldown = 6f;

    [SerializeField] private float attackRange = 2f;

    [SerializeField] private bool canRangeAttack = true;
    [SerializeField] private bool isRangeAttaking = false;

    [SerializeField] GameObject projectilePrefab;

    [SerializeField] Transform projectileSpawnTransform;

    public float RangeAttackCooldown { get => rangeAttackCooldown; set => rangeAttackCooldown = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public bool CanRangeAttack { get => canRangeAttack; set => canRangeAttack = value; }
    public bool IsRangeAttaking { get => isRangeAttaking; set => isRangeAttaking = value; }

    public EnemyRangeAttackState() : base()
    {
        stateEnum = EnemyStateEnum.RangeAttack;

    }

    public override void OnEnterState()
    {
        StartCoroutine(RangeAttackCoroutine());
    }

    public override void OnExitState()
    {
        IsRangeAttaking = false;
        enemyController.CanMove = true;
    }

    public override void HandleState()
    {


    }

    private IEnumerator RangeAttackCoroutine()
    {
        CanRangeAttack = false;
        IsRangeAttaking = true;
        enemyController.CanMove = false;

        GameObject obj = Instantiate(projectilePrefab, projectileSpawnTransform.position,Quaternion.identity);
        Projectile proj = obj.GetComponent<Projectile>();
        proj.EnemyController = enemyController;
        proj.SetVelocity(enemyController.player.transform.position);

        yield return new WaitForSeconds(0.25f);
        enemyController.ChangeState(EnemyStateEnum.Idle);

        yield return new WaitForSeconds(rangeAttackCooldown);
        CanRangeAttack = true;
        //enemyController.CanMove = true;
    }
}
