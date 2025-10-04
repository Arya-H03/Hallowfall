using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private EnemySignalHub signalHub;
    public EnemyDeathState(EnemyController enemyController, EnemyStateMachine stateMachine, EnemyStateEnum stateEnum) : base(enemyController, stateMachine, stateEnum)
    {
        this.enemyController = enemyController;
        this.enemyConfig = enemyController.EnemyConfig;      
        signalHub = enemyController.SignalHub;
       
    }
    public override void EnterState()
    {
        enemyController.IsDead = true;      
        enemyController.CoroutineRunner.StartCoroutine(HandleDeathCoroutine());
    }

    private IEnumerator HandleDeathCoroutine()
    {
        signalHub.OnResetAnimTrigger?.Invoke("Hit");
        signalHub.OnAnimTrigger?.Invoke("Death");

        signalHub.OnDeActivateHealthbar?.Invoke();
        signalHub.OnDisablePhysicsAndCollision?.Invoke();

        signalHub.OnEnemyDeathBegin?.Invoke();
        PlayerHealthPotionHandler.Instance.OnPotionChargeRestored?.Invoke(5);
        signalHub.OnItemDrop?.Invoke();

        float deathAnimDuration = (float)signalHub.RequestAnimLength?.Invoke("Death");
        yield return new WaitForSeconds(deathAnimDuration);

       

        DeSpawnEnemy();

    }
    private void DeSpawnEnemy()
    {
        signalHub.OnEnemyDeathEnd?.Invoke();

        ReturnEnemyToPool();

        enemyController.IsDead = false;
        signalHub.OnRestoreFullHealth?.Invoke();
        signalHub.OnActivateHealthbar?.Invoke();
        signalHub.OnEnablePhysicsAndCollision?.Invoke();
        stateMachine.ChangeState(EnemyStateEnum.Idle);

    }

    private void ReturnEnemyToPool()
    {
        switch (enemyController.EnemyType)
        {
            case EnemyTypeEnum.Arsonist:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(enemyController.gameObject);
                break;
            case EnemyTypeEnum.Revenant:
                ObjectPoolManager.Instance.RevenantPool.ReturnToPool(enemyController.gameObject);
                break;
            case EnemyTypeEnum.Sinner:
                ObjectPoolManager.Instance.SinnerPool.ReturnToPool(enemyController.gameObject);
                break;
            case EnemyTypeEnum.Necromancer:
                ObjectPoolManager.Instance.ArsonistPool.ReturnToPool(enemyController.gameObject);
                break;
            case EnemyTypeEnum.Spectrum:
                ObjectPoolManager.Instance.SpectrumPool.ReturnToPool(enemyController.gameObject);
                break;
            case EnemyTypeEnum.Undead:
                ObjectPoolManager.Instance.UndeadPool.ReturnToPool(enemyController.gameObject);
                break;
        }
    }
}
