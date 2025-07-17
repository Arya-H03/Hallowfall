using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerBaseState
{

    private float moveSpeedWhileAttaking = 0;
    private float hitStopDuration = 0;
    private float attackComboWindow = 0;
    private float attackDelay = 0;


    private bool canAttack = true;
    private bool isInCombo = false;
    private int comboIndex = 0;
    private Coroutine comboResetCoroutine;


    private AudioSource audioSource;
    private AudioClip[] attackSwingSFX;


    public delegate void EventHandler();
    public EventHandler OnFirstSwordSwingEvent;
    public EventHandler OnSecondSwordSwingEvent;
    public EventHandler OnThirdSwordSwingEvent;


    private Coroutine SpawnAfterImageCoroutine;
    [SerializeField] private GameObject firstSwingEffect;
    [SerializeField] private GameObject secondSwingEffect;


    private float firstSwingDamage = 0;
    private float secondSwingDamage = 0;
    private float thirdSwingDamage = 0;


    private LayerMask layerMask;
    private float distance;

    [SerializeField] private Transform firstSwingCenter;
    private Vector2 firstSwingCastSize = Vector2.zero;

    [SerializeField] private Transform secondSwingCenter;
    private Vector2 secondSwingCastSize = Vector2.zero;

    [SerializeField] private Transform thirdSwingCenter;
    private Vector2 thirdSwingCastSize = Vector2.zero;

    public Transform FirstSwingCenter => firstSwingCenter;
    public Transform SecondSwingCenter => secondSwingCenter;
    public Transform ThirdSwingCenter => thirdSwingCenter;

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();
        OnFirstSwordSwingEvent += FirstSwingBoxCast;
        OnSecondSwordSwingEvent += SecondSwingBoxCast;
        OnThirdSwordSwingEvent += ThirdSwingBoxCast;
    }

    public override void InitState(PlayerConfig config)
    {
        moveSpeedWhileAttaking = config.moveSpeedWhileAttaking;
        hitStopDuration = config.hitStopDuration;
        attackComboWindow = config.attackComboWindow;
        attackDelay = config.attackDelay;

        attackSwingSFX = config.attackSwingSFX;

        firstSwingDamage = config.firstSwingDamage;
        secondSwingDamage = config.secondSwingDamage;
        thirdSwingDamage = config.thirdSwingDamage;

        layerMask = config.layerMask;
        distance = config.distance;

        firstSwingCastSize = config.firstSwingCastSize;
        secondSwingCastSize = config.secondSwingCastSize;
        thirdSwingCastSize = config.thirdSwingCastSize;
    }

    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileAttaking;
        playerController.IsAttacking = true;
    }

    public override void OnExitState()
    {
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;
        playerController.PlayerMovementManager.TurnPlayer(playerController.PlayerMovementManager.currentInputDir);
    }

    public override void HandleState()
    {
        
    }


    public void HandleAttack()
    {
        if (!canAttack) return;

        comboIndex++;
        if (comboIndex > 2) comboIndex = 1;

        playerController.PlayerMovementManager.TurnPlayerWithMousePos();

        PlaySwingAnimation(comboIndex);

        canAttack = false;
        isInCombo = true;

        StartCoroutine(AttackCooldown());

        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);
        comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(attackComboWindow);
        comboIndex = 0;
        isInCombo = false;
    }

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

        AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, attackSwingSFX.Length)], 1);
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
        if (playerController.PlayerMovementManager.currentInputDir != Vector2.zero)
            playerController.ChangeState(PlayerStateEnum.Run);
        else
            playerController.ChangeState(PlayerStateEnum.Idle);
    }


    private void FirstSwingBoxCast()
    {
        RaycastHit2D[] hitResult = BoxCastForAttack(firstSwingCenter.position, firstSwingCastSize, 1);
        HandleHits(hitResult, firstSwingDamage,1);
    }

    private void SecondSwingBoxCast()
    {
        RaycastHit2D[] hitResult = BoxCastForAttack(secondSwingCenter.position, secondSwingCastSize, 2);
        HandleHits(hitResult, secondSwingDamage,3);
    }

    public void ThirdSwingBoxCast()
    {
        RaycastHit2D[] hitResult = BoxCastForAttack(thirdSwingCenter.position, thirdSwingCastSize, 3);
        HandleHits(hitResult, thirdSwingDamage, 5);
    }

    private RaycastHit2D[] BoxCastForAttack(Vector2 centerPoint, Vector2 boxSize, int index)
    {
        Vector2 direction = transform.right;
        GameObject slashEffectGO = SpawnSlashEffect(index);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(slashEffectGO.transform.position, 1.2f, direction, distance, layerMask);
        return hits.Length > 0 ? hits : null;
    }

    private void HandleHits(RaycastHit2D[] hits, float damage,float force)
    {
        if (hits == null) return;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyController enemyController = hit.collider.GetComponent<EnemyController>();
                Vector2 knockbackVector = (playerController.GetPlayerCenter() - enemyController.GetEnemyCenter()).normalized;
                enemyController.OnEnemyHit(damage, hit.point, HitSfxType.sword, force);
                playerController.PlayerCollision.KnockBackPlayer(knockbackVector, 0.25f);
                GameManager.Instance.StopTime(hitStopDuration);
            }
        }
    }

    private GameObject SpawnSlashEffect(int index)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dir = (mousePos - playerController.GetPlayerCenter()).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject effect = null;

        if (index == 1)
        {
            effect = Instantiate(firstSwingEffect, playerController.GetPlayerCenter(), Quaternion.identity);
            angle -= 60;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (index == 2)
        {
            effect = Instantiate(secondSwingEffect, playerController.GetPlayerCenter(), Quaternion.identity);
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
