using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerBaseState
{
    private EnemyDetector enemyDetector;

    private float moveSpeedWhileAttaking = 0;
    private float hitStopDuration = 0;

    private float swingComboWindow = 0;
    private float delayBetweenSwings = 0;
    private float swingInputLifeTime = 0;


    //private bool canAttack = true;

    //private bool isInCombo = false;
    private int comboIndex = 0;

    private Coroutine comboResetCoroutine;

    private int attackIndex = 1;
    private List<int> listOfqueuedAttacks = new();

    private AudioClip[] attackSwingSFX;


    public delegate void EventHandler();
    public EventHandler OnFirstSwordSwingEvent;
    public EventHandler OnSecondSwordSwingEvent;
    public EventHandler OnThirdSwordSwingEvent;


    private Coroutine SpawnAfterImageCoroutine;
    [SerializeField] private GameObject firstSwingEffect;
    [SerializeField] private GameObject secondSwingEffect;
    [SerializeField] private GameObject hitSparkPrefab;


    private float firstSwingDamage = 0;
    private float secondSwingDamage = 0;
    private float thirdSwingDamage = 0;


    private LayerMask layerMask;
    private float distance;

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    public override void Start()
    {
        base.Start();
        enemyDetector = playerController.EnemyDetector;
        OnFirstSwordSwingEvent += FirstSwingBoxCast;
        OnSecondSwordSwingEvent += SecondSwingBoxCast;
        OnThirdSwordSwingEvent += ThirdSwingBoxCast;
    }

    public override void InitState(PlayerConfig config)
    {
        moveSpeedWhileAttaking = config.moveSpeedWhileAttaking;

        hitStopDuration = config.hitStopDuration;

        swingComboWindow = config.swingComboWindow;
        swingInputLifeTime = config.swingInputLifeTime;
        delayBetweenSwings = config.delayBetweenSwings;

        attackSwingSFX = config.attackSwingSFX;

        firstSwingDamage = config.firstSwingDamage;
        secondSwingDamage = config.secondSwingDamage;
        thirdSwingDamage = config.thirdSwingDamage;

        layerMask = config.layerMask;
        distance = config.distance;

    }

    public override void OnEnterState()
    {
        playerController.PlayerMovementHandler.MoveSpeed = moveSpeedWhileAttaking;
       
    }

    public override void OnExitState()
    {
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;
        ClearAllQueuedAttacks();
        playerController.PlayerMovementHandler.TurnPlayer(playerController.PlayerMovementHandler.currentInputDir);
    }

    public override void HandleState()
    {

    }


    public void HandleAttack()
    {
        listOfqueuedAttacks.Add(attackIndex);
        StartCoroutine(RemoveAttackFromQueue(listOfqueuedAttacks, attackIndex));
        attackIndex++;

        if (!playerController.IsAttacking)
        {
            StartCoroutine(SwingCoroutine(listOfqueuedAttacks));
        }
    }

    private IEnumerator SwingCoroutine(List<int> QueuedAttacks)
    {
        if (playerController.CurrentStateEnum == PlayerStateEnum.SwordAttack)
        {
            playerController.IsAttacking = true;

            int index = listOfqueuedAttacks[0];
            listOfqueuedAttacks.Remove(index);

            comboIndex++;
            if (comboIndex > 2) comboIndex = 1;
            //isInCombo = true;

            playerController.PlayerMovementHandler.TurnPlayerWithMousePos();

            PlaySwingAnimation(comboIndex);

            yield return new WaitForSeconds(delayBetweenSwings);
            playerController.IsAttacking = false;

            if (comboResetCoroutine != null) StopCoroutine(comboResetCoroutine);

            comboResetCoroutine = StartCoroutine(ResetSwingComboCoroutine());

            if (QueuedAttacks.Count > 0) StartCoroutine(SwingCoroutine(QueuedAttacks));
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
        //isInCombo = false;
    }

    private void ClearAllQueuedAttacks()
    {
        listOfqueuedAttacks.Clear();
        attackIndex = 1;
    }

    //public void HandleAttack()
    //{
    //    if (!canAttack) return;

    //    comboIndex++;
    //    if (comboIndex > 2) comboIndex = 1;

    //    playerController.PlayerMovementHandler.TurnPlayerWithMousePos();

    //    PlaySwingAnimation(comboIndex);

    //    canAttack = false;
    //    isInCombo = true;

    //    StartCoroutine(AttackCooldown());

    //    if (comboResetCoroutine != null) StopCoroutine(comboResetCoroutine);

    //    comboResetCoroutine = StartCoroutine(ResetSwingComboCoroutine());
    //}
    //private IEnumerator AttackCooldown()
    //{
    //    yield return new WaitForSeconds(swingInputLifeTime);
    //    canAttack = true;
    //}

  
    private void PlaySwingAnimation(int comboIndex)
    {
        switch (comboIndex)
        {
            case 1:
                playerController.AnimationController.SetTriggerForAnimations("FirstSwing");
                break;
            case 2:
                playerController.AnimationController.SetTriggerForAnimations("SecondSwing");
                break;
            case 3:
                playerController.AnimationController.SetTriggerForAnimations("ThirdSwing");
                break;
        }

        AudioManager.Instance.PlaySFX(attackSwingSFX[Random.Range(0, attackSwingSFX.Length)], playerController.transform.position, 0.5f);
    }

    public void HandleFirstSwing()
    {
        playerController.IsAttacking = true;
        playerController.CanPlayerAttack = false;
        playerController.AnimationController.SetTriggerForAnimations("Attack");
    }

    public void HandleDoubleSwing()
    {
        playerController.AnimationController.SetTriggerForAnimations("DoubleSwing");
    }

    public void HandleJumpAttack()
    {
        playerController.CanPlayerAttack = false;
        playerController.AnimationController.SetTriggerForAnimations("JumpAttack");
    }

    public void EndAttack()
    {

        if (playerController.PlayerMovementHandler.currentInputDir != Vector2.zero)
            playerController.ChangeState(PlayerStateEnum.Run);
        else
            playerController.ChangeState(PlayerStateEnum.Idle);
    }


    private void FirstSwingBoxCast()
    {
        HandleHits(enemyDetector.AvailableEnemyTargets, firstSwingDamage, 1);
        SpawnSlashEffect(1);
    }

    private void SecondSwingBoxCast()
    {
        HandleHits(enemyDetector.AvailableEnemyTargets, secondSwingDamage, 2);
        SpawnSlashEffect(2);
    }

    public void ThirdSwingBoxCast()
    {
        HandleHits(enemyDetector.AvailableEnemyTargets, thirdSwingDamage, 3);
    }

    private RaycastHit2D[] BoxCastForAttack(Vector2 centerPoint, Vector2 boxSize, int index)
    {
        Vector2 direction = transform.right;
        GameObject slashEffectGO = SpawnSlashEffect(index);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(slashEffectGO.transform.position, 1.2f, direction, distance, layerMask);
        return hits.Length > 0 ? hits : null;
    }

    private void HandleHits(HashSet<EnemyController> enemies, float damage, float force)
    {
        if (enemies == null || enemies.Count < 1) return;

        foreach (EnemyController enemy in enemies)
        {
            Vector2 dirVectorFromPlayerToEnemy = (playerController.GetPlayerPos() - enemy.GetEnemyPos()).normalized;
            enemy.HitEnemy(damage, enemy.transform.position, HitSfxType.sword, force);
            SpawnHitEffects(enemy, dirVectorFromPlayerToEnemy);
               
            //playerController.PlayerPhysicsController.KnockBackPlayer(knockbackVector, 0.1f);
            GameManager.Instance.StopTime(hitStopDuration);
        }
    }

    private void SpawnHitEffects(EnemyController enemyController, Vector2 dir)
    {
        Vector3 randPos = new Vector3(Random.Range(-0.25f,0.25f), Random.Range(-0.25f, 0.25f),0);
        GameObject hitSpark = Instantiate(hitSparkPrefab, enemyController.GetEnemyPos() + randPos, Quaternion.identity);
        Vector3 scale = hitSpark.transform.localScale;
        hitSpark.transform.localScale = dir.x < 0 ? new Vector3(Mathf.Abs(scale.x),scale.y,scale.z) : new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
    }

    private GameObject SpawnSlashEffect(int index)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dir = (mousePos - playerController.GetPlayerPos()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject effect = null;

        if (index == 1)
        {
            effect = Instantiate(firstSwingEffect, playerController.GetPlayerPos(), Quaternion.identity);
            angle -= 60;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (index == 2)
        {
            effect = Instantiate(secondSwingEffect, playerController.GetPlayerPos(), Quaternion.identity);
            if (angle <= -90 || angle >= 90)
            {
                SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
                sr.flipX = true;
                sr.flipY = true;
            }
            effect.transform.rotation = Quaternion.Euler(65, 0, angle);
        }

        effect.transform.position += dir;
        return effect;
    }
}
