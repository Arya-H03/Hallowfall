using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSwordAttackState : PlayerState
{
    private EnemyDetector enemyDetector;

    private float moveSpeedWhileAttaking = 0;
    private float hitStopDuration = 0;

    private float swingComboWindow = 0;
    private float delayBetweenSwings = 0;
    private float swingInputLifeTime = 0;


    private int comboIndex = 0;

    private Coroutine comboResetCoroutine;

    private int attackIndex = 1;
    private List<int> listOfqueuedAttacks = new();

    private AudioClip[] attackSwingSFX;

     private GameObject firstSwingEffect;
     private GameObject secondSwingEffect;
     private GameObject hitSparkPrefab;


    private float firstSwingDamage = 0;
    private float secondSwingDamage = 0;

    public PlayerSwordAttackState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;

        enemyDetector = playerController.EnemyDetector;

        signalHub.OnFirstSwordSwing += FirstSwingBoxCast;
        signalHub.OnSecondSwordSwing += SecondSwingBoxCast;
        signalHub.OnSwordSwingEnd += EndAttack;

        this.firstSwingEffect = playerConfig.firstSwingEffect;
        this.secondSwingEffect = playerConfig.secondSwingEffect;
        hitSparkPrefab = playerConfig.hitEffect;

        moveSpeedWhileAttaking = playerConfig.moveSpeedWhileAttaking;

        hitStopDuration = playerConfig.hitStopDuration;

        swingComboWindow = playerConfig.swingComboWindow;
        swingInputLifeTime = playerConfig.swingInputLifeTime;
        delayBetweenSwings = playerConfig.delayBetweenSwings;

        attackSwingSFX = playerConfig.attackSwingSFX;

        firstSwingDamage = playerConfig.firstSwingDamage;
        secondSwingDamage = playerConfig.secondSwingDamage;

    }
    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        playerController.IsAttacking = false;
        playerController.CanAttack = true;
        ClearAllQueuedAttacks();
    }
    public void HandleAttack()
    {
        listOfqueuedAttacks.Add(attackIndex);
        playerController.CoroutineRunner.RunCoroutine(RemoveAttackFromQueue(listOfqueuedAttacks, attackIndex));
        attackIndex++;

        if (!playerController.IsAttacking)
        {
            playerController.CoroutineRunner.RunCoroutine(SwingCoroutine(listOfqueuedAttacks));
        }
    }

    private IEnumerator SwingCoroutine(List<int> QueuedAttacks)
    {
        if (playerController.StateMachine.CurrentStateEnum == PlayerStateEnum.SwordAttack)
        {
            playerController.IsAttacking = true;

            int index = listOfqueuedAttacks[0];
            listOfqueuedAttacks.Remove(index);

            comboIndex++;
            if (comboIndex > 2) comboIndex = 1;
           

            signalHub.OnTurningToMousePos?.Invoke();

            PlaySwingAnimation(comboIndex);

            signalHub.OnPlayRandomSFX?.Invoke(attackSwingSFX,0.5f);

            yield return new WaitForSeconds(delayBetweenSwings);
            playerController.IsAttacking = false;

            if (comboResetCoroutine != null) playerController.CoroutineRunner.EndCoroutine(comboResetCoroutine);

            comboResetCoroutine = playerController.CoroutineRunner.RunCoroutine(ResetSwingComboCoroutine());

            if (QueuedAttacks.Count > 0) playerController.CoroutineRunner.RunCoroutine(SwingCoroutine(QueuedAttacks));
        }

    }

  
    private IEnumerator RemoveAttackFromQueue(List<int> attackQueue, int attackIndex)
    {
        yield return new WaitForSeconds(swingInputLifeTime);
        if (attackQueue.Contains(attackIndex))
        {
            attackQueue.Remove(attackIndex);
        }

    }

    private IEnumerator ResetSwingComboCoroutine()
    {
        yield return new WaitForSeconds(swingComboWindow);
        comboIndex = 0;       
    }

    private void ClearAllQueuedAttacks()
    {
        listOfqueuedAttacks.Clear();
        attackIndex = 1;
    }
    private void PlaySwingAnimation(int comboIndex)
    {
        switch (comboIndex)
        {
            case 1:
                signalHub.OnAnimTrigger?.Invoke("FirstSwing");
                break;
            case 2:
                signalHub.OnAnimTrigger?.Invoke("SecondSwing");
                break;
            case 3:
                signalHub.OnAnimTrigger?.Invoke("ThirdSwing");
                break;
        }
      
    }

    private void EndAttack()
    {
        signalHub.OnStateTransitionBasedOnMovement?.Invoke(PlayerStateEnum.SwordAttack);
    }


    private void FirstSwingBoxCast()
    {
        //SpawnSlashEffect(1);
        HandleHits(enemyDetector.AvailableEnemyTargets, firstSwingDamage, 1);
       
    }

    private void SecondSwingBoxCast()
    {
        //SpawnSlashEffect(2);
        HandleHits(enemyDetector.AvailableEnemyTargets, secondSwingDamage, 2);
        
    }

    private void HandleHits(HashSet<EnemyController> enemies, float damage, float force)
    {
        if (enemies == null || enemies.Count < 1) return;

        foreach (EnemyController enemy in enemies)
        {
            Vector2 dirVectorFromPlayerToEnemy = (playerController.GetPlayerPos() - enemy.GetEnemyPos()).normalized;
            enemy.EnemyHitHandler.TryHitEnemy(damage, HitSfxType.sword, force);
            SpawnHitEffects(enemy, dirVectorFromPlayerToEnemy);

            //playerController.PlayerPhysicsHandler.KnockBackPlayer(knockbackVector, 0.1f);
            GameManager.Instance.StopTime(hitStopDuration);
        }
    }

    private void SpawnHitEffects(EnemyController enemyController, Vector2 dir)
    {
        Vector3 randPos = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
        Vector3 originScale = hitSparkPrefab.transform.localScale;
        Vector3 newScale = dir.x < 0 ? new Vector3(Mathf.Abs(originScale.x), originScale.y, originScale.z) : new Vector3(-Mathf.Abs(originScale.x), originScale.y, originScale.z);
        signalHub.OnSpawnScaledVFX?.Invoke(hitSparkPrefab, enemyController.GetEnemyPos() + randPos, Quaternion.identity, 2, newScale);  
    }

    private void SpawnSlashEffect(int index)
    {
        Vector3 mousePos = MyUtils.GetMousePos();
        Vector3 dir = (mousePos - playerController.GetPlayerPos()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //GameObject effect = null;

        if (index == 1)
        {
            signalHub.OnSpawnVFX?.Invoke(firstSwingEffect, playerController.GetPlayerPos() + dir, Quaternion.Euler(0, 0, angle - 60),2);
            //effect = Instantiate(firstSwingEffect, playerController.GetPlayerPos(), Quaternion.identity);
            //angle -= 60;
            //effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (index == 2)
        {
            signalHub.OnSpawnVFX?.Invoke(firstSwingEffect, playerController.GetPlayerPos() + dir, Quaternion.Euler(0, 0, angle - 60), 2);
            //signalHub.OnSpawnVFX?.Invoke(secondSwingEffect, playerController.GetPlayerPos() + dir, Quaternion.Euler(65, 180, angle + 180), 2);
            //effect = Instantiate(secondSwingEffect, playerController.GetPlayerPos(), Quaternion.identity);
            //if (angle > -90 && angle < 90)
            //{
            //    SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
            //    sr.flipX = false;
            //    sr.flipY = false;
            //}
            //effect.transform.rotation = Quaternion.Euler(65, 0, angle);
        }

        //effect.transform.position += dir;

    }
}
