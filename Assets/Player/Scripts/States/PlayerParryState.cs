using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryState : PlayerBaseState
{
    #region Fields and Properties

    [Header("Parry Settings")]
    [SerializeField] private float moveSpeedWhileParrying = 2f;

    [Header("References")]
    [SerializeField] private GameObject parryShield;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private AudioClip parrySFX;

    private AudioSource audioSource;
    private bool canCounterParry = false;
    private bool canParryProjectiles = false;

    public bool CanCounterParry => canCounterParry;

    public bool CanParryProjectiles { get => canParryProjectiles; set => canParryProjectiles = value; }

    public static event Action OnParrySuccessful;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SkillEvents.OnCounterSkillUnlocked += CounterSkillLogic;
        SkillEvents.OnEchoingSteelSkillUnlocked += EchoingSteelSkillLogic;
        SkillEvents.OnMomentumShiftSkillUnlocked += MonentumShiftSkillLogic;
        SkillEvents.OnCounterSurgeSkillUnlocked += CounterSurgeLogic;

        OnParrySuccessful += CreateEffectsOnParrySuccess;
    }

    private void OnDisable()
    {
        SkillEvents.OnCounterSkillUnlocked -= CounterSkillLogic;
        SkillEvents.OnEchoingSteelSkillUnlocked -= EchoingSteelSkillLogic;
        SkillEvents.OnMomentumShiftSkillUnlocked -= MonentumShiftSkillLogic;
        SkillEvents.OnCounterSurgeSkillUnlocked -= CounterSurgeLogic;

        OnParrySuccessful -= CreateEffectsOnParrySuccess;
        OnParrySuccessful -= EchoingSteel;
        OnParrySuccessful -= MonentumShift;
        OnParrySuccessful -= CounterSurge;
    }

    #endregion

    #region State Methods

    public PlayerParryState()
    {
        this.stateEnum = PlayerStateEnum.Parry;
    }

    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileParrying;
        StartParry();
    }

    public override void OnExitState()
    {
        // Revert to default move speed if needed
        playerController.PlayerMovementManager.MoveSpeed = playerController.PlayerMovementManager.MoveSpeed;
    }

    public override void HandleState()
    {
        // Optional runtime behavior
    }

    #endregion

    #region Parry Logic

    private void StartParry()
    {
        playerController.IsParrying = true;
        playerController.CanPlayerJump = false;
        playerController.AnimationController.SetTriggerForAnimations("Parry");
        playerController.AnimationController.SetBoolForAnimations("isParrying", true);
        StartCoroutine(StopParryCoroutine());
    }

    private IEnumerator StopParryCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        playerController.AnimationController.SetBoolForAnimations("isParrying", false);
        OnParryEnd();
    }

    public void OnParryEnd()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = false;
        playerController.IsParrying = false;
        playerController.CanPlayerJump = true;
        playerController.AnimationController.SetBoolForAnimations("isParrySuccessful", false);

        var horizontalInput = playerController.PlayerMovementManager.currentInputDir.x;
        playerController.ChangeState(horizontalInput != 0 ? PlayerStateEnum.Run : PlayerStateEnum.Idle);
    }

    public void ActivateParryShield()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void CallOnParrySuccessfulEvent()
    {
        OnParrySuccessful?.Invoke();
    }

    private void CreateEffectsOnParrySuccess()
    {
        playerController.AnimationController.SetBoolForAnimations("isParrySuccessful", true);
        AudioManager.Instance.PlaySFX(audioSource, parrySFX, 1f);
    }

    #endregion

    #region Skills

    private void CounterSkillLogic()
    {
        canCounterParry = true;
    }

    private void EchoingSteelSkillLogic()
    {
        OnParrySuccessful += EchoingSteel;
    }

    private void EchoingSteel()
    {
        var center = playerController.GetPlayerCenter();
        RaycastHit2D[] enemies = Physics2D.CircleCastAll(center, 3f, Vector2.zero, 0, playerController.EnemyLayer);

        foreach (var enemy in enemies)
        {
            if (enemy.collider.CompareTag("Enemy"))
            {
                enemy.collider.GetComponent<EnemyController>()
                    .collisionManager.StaggerEnemy(100);
            }
        }
    }

    private void MonentumShiftSkillLogic()
    {
        OnParrySuccessful += MonentumShift;
    }

    private void MonentumShift()
    {
        StartCoroutine(MonentumShiftCoroutine());
    }

    private IEnumerator MonentumShiftCoroutine()
    {
        playerController.PlayerMovementManager.SpeedModifer = 1.25f;
        yield return new WaitForSeconds(3f);
        playerController.PlayerMovementManager.SpeedModifer = 1f;

    }

    private void CounterSurgeLogic()
    {
        OnParrySuccessful += CounterSurge;
    }

    private void CounterSurge()
    {
        playerController.RestoreHealth((int)playerController.MaxHealth / 5);
    }
    #endregion

    #region Utility

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj = Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj, 0.5f);
    }

    public bool CanCounter()
    {
        return CanCounterParry;
    }

    #endregion
}
