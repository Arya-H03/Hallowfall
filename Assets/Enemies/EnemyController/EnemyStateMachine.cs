using UnityEngine;

public class EnemyStateMachine
{
    private EnemyController enemyController;

    private EnemyIdleState idleState;
    private EnemyChaseState chaseState;
    private EnemyAttackState attackState;
    private EnemyStunState stunState;
    private EnemyDeathState deathState;
    private EnemyPatrolState patrolState;


    private bool canChangeState = true;

    private EnemyState currentState;
    private EnemyStateEnum currentStateEnum;
    public EnemyState CurrentState { get => currentState;}
    public EnemyIdleState IdleState { get => idleState;}
    public EnemyChaseState ChaseState { get => chaseState;}
    public EnemyAttackState AttackState { get => attackState;}
    public EnemyStunState StunState { get => stunState; }
    public EnemyDeathState DeathState { get => deathState;}
    public EnemyPatrolState PatrolState { get => patrolState; }
    public bool CanChangeState { get => canChangeState;}
    public EnemyStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }


    public EnemyStateMachine(EnemyController enemyController)
    {
        this.enemyController = enemyController;
    }
   
    public void InitAllStates()
    {
        idleState = new EnemyIdleState(enemyController, this, EnemyStateEnum.Idle);
        chaseState = new EnemyChaseState(enemyController, this, EnemyStateEnum.Chase);
        attackState = new EnemyAttackState(enemyController, this, EnemyStateEnum.Attack);
        stunState = new EnemyStunState(enemyController, this, EnemyStateEnum.Stun);
        deathState = new EnemyDeathState(enemyController, this, EnemyStateEnum.Death);
        patrolState = new EnemyPatrolState(enemyController, this, EnemyStateEnum.Patrol);

        currentState =  idleState;
        currentStateEnum = EnemyStateEnum.Idle;
    }
    public void ChangeState(EnemyStateEnum stateEnum)
    {
        if (currentStateEnum != stateEnum && canChangeState && !enemyController.IsDead)
        {
            //Debug.Log(CurrentStateEnum.ToString() + " to " + stateEnum.ToString());
            CurrentState?.ExitState();
            switch (stateEnum)
            {
                case EnemyStateEnum.Idle:
                    currentState = idleState;
                    break;
                case EnemyStateEnum.Chase:
                    currentState = chaseState;
                    break;
                case EnemyStateEnum.Attack:
                    currentState = attackState;
                    break;
                case EnemyStateEnum.Stun:
                    currentState = stunState;
                    break;
                case EnemyStateEnum.Death:
                    currentState = deathState;
                    break;
                case EnemyStateEnum.Patrol:
                    currentState = patrolState;
                    break;
            }
            currentStateEnum = stateEnum;
            currentState.EnterState();
        }
    }
}
