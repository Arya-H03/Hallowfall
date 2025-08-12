using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public enum ComboState
{
    None,
    Attack1,
    Attack2,
    Attack3
}
[System.Serializable]
public struct ComboAttack
{
    public ComboState comboState;
    public float attackDamge;
    public float knockbackForce;
    public GameObject slashEffectPrefab;
}
public class PlayerSwordAttackState : PlayerState
{
    private EnemyDetector enemyDetector;

    private float moveSpeedWhileAttaking = 0;
    private float hitStopDuration = 0;

    private Dictionary<ComboState,ComboAttack> combosDict = new Dictionary<ComboState,ComboAttack>();
    private ComboState currentComboState = ComboState.None;
    private ComboAttack currentAttack;

    private float swingComboWindow;
    private float lastAttackTime;
    private bool isWithinCombo = false;
    private bool isNextAttackQueued = false;

    private AudioClip[] attackSwingSFX;

     private GameObject firstSwingEffect;
     private GameObject secondSwingEffect;
     private GameObject hitSparkPrefab;

    public PlayerSwordAttackState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;

        enemyDetector = playerController.EnemyDetector;

        signalHub.OnSwordAttackHitFrame += HandleHittingTarget;
        signalHub.OnSwordAttackSFXFrame += () => { signalHub.OnPlayRandomSFX?.Invoke(attackSwingSFX, 0.5f); };

        signalHub.OnSwordSwingEnd += OnAttackAnimationComplete;
        signalHub.OnParryAttackHit += HandleHittingTargetForPattayAttack;

        this.firstSwingEffect = playerConfig.firstSwingEffect;
        this.secondSwingEffect = playerConfig.secondSwingEffect;
        hitSparkPrefab = playerConfig.hitEffect;
        InitializeComboData(playerConfig.comboAttacks);
        moveSpeedWhileAttaking = playerConfig.moveSpeedWhileAttaking;

        hitStopDuration = playerConfig.hitStopDuration;

        swingComboWindow = playerConfig.swingComboWindow;
        attackSwingSFX = playerConfig.attackSwingSFX;

    }

    private void InitializeComboData(List<ComboAttack> comboAttacksList)
    {
        foreach (ComboAttack comboAttack in comboAttacksList)
        {
            if (!combosDict.ContainsKey(comboAttack.comboState)) combosDict.Add(comboAttack.comboState, comboAttack);
        }
    }
    #region State Events


    public override void FrameUpdate()
    {
        if (Time.time - lastAttackTime > swingComboWindow)
        {
            ResetCombo();
        }

        if (isNextAttackQueued && !playerController.IsAttacking)
        {
            isNextAttackQueued = false;
            TryCombo();
        }
    }

    public override void ExitState()
    {
        ResetCombo();
    }
    #endregion

    #region Combo Logic

    public void TrySwordAttack()
    {
        if (playerController.IsAttacking)
        {
           isNextAttackQueued = true;
        }
        else
        {
            TryCombo();
        }
    }

    private void TryCombo()
    {
        if (playerController.IsAttacking) return;

        playerController.IsAttacking = true;
        lastAttackTime = Time.time;
        isWithinCombo = true;

        signalHub.OnTurningToMousePos?.Invoke();

        switch (currentComboState)
        {
            case ComboState.None:
                currentComboState = ComboState.Attack1;
                signalHub.OnAnimTrigger?.Invoke("FirstSwing");
                break;
            case ComboState.Attack1:
                currentComboState = ComboState.Attack2;
                signalHub.OnAnimTrigger?.Invoke("SecondSwing");
                break;
            case ComboState.Attack2:
                currentComboState = ComboState.Attack3;
                signalHub.OnAnimTrigger?.Invoke("ThirdSwing");
                break;
        }

        currentAttack = combosDict[currentComboState];
    }

    public void OnAttackAnimationComplete()
    {
        playerController.IsAttacking = false;
  
        if (currentComboState == ComboState.Attack3 || !isWithinCombo || !isNextAttackQueued)
        {
            ResetCombo();

            signalHub.OnStateTransitionBasedOnMovement?.Invoke(PlayerStateEnum.SwordAttack);
        }
    }

    private void ResetCombo()
    {
        isWithinCombo = false;
        isNextAttackQueued = false;
        currentComboState = ComboState.None;
        currentAttack = combosDict[currentComboState];
        playerController.IsAttacking = false;
    }

    #endregion
 
    private void HandleHittingTarget()
    {
        //SpawnSlashEffect(1);
        //signalHub.OnSpawnVFX?.Invoke(currentAttack.slashEffectPrefab, playerController.GetPlayerPos() +  new Vector3(0, 1, 0), Quaternion.identity, 3);
        TryHit(enemyDetector.AvailableEnemyTargets, currentAttack.attackDamge, currentAttack.knockbackForce);
       
    }
    private void HandleHittingTargetForPattayAttack()
    {
        TryHit(enemyDetector.AvailableEnemyTargets, combosDict[ComboState.Attack2].attackDamge, combosDict[ComboState.Attack2].knockbackForce);
    }

    private void TryHit(HashSet<EnemyController> enemies, float damage, float force)
    {
        if (enemies == null || enemies.Count < 1) return;

        foreach (EnemyController enemy in enemies)
        {
            Vector2 dirVectorFromPlayerToEnemy = (playerController.GetPlayerPos() - enemy.GetEnemyPos()).normalized;
            enemy.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = damage, HitSfx = HitSfxType.sword, AttackerPosition = playerController.GetPlayerPos(), KnockbackForce = force });

            SpawnHitEffects(enemy, dirVectorFromPlayerToEnemy);

            //playerController.PlayerPhysicsHandler.KnockBackPlayer(knockbackVector, 0.1f);
            
        }
        GameManager.Instance.StopTime(hitStopDuration);
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
