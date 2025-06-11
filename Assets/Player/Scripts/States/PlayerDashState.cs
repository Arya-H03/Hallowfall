using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState
{

    private DashAttackBox dashAttackBox;
    private float dashAttackDamage;
    private float dashModifier;
    private float dashAttackDelay;
    private AudioClip[] dashAttackSFX;

    private Coroutine afterImageCoroutine;
    private AudioSource audioSource;

    public float DashAttackDamage { get => dashAttackDamage; set => dashAttackDamage = value; }

    public PlayerDashState()
    {
        this.stateEnum = PlayerStateEnum.Dash;
    }

    public override void InitState(PlayerConfig config)
    {
        DashAttackDamage = config.dashAttackDamage;
        dashModifier = config.dashModifier;
        dashAttackDelay = config.dashAttackDelay;
        dashAttackSFX = config.dashAttackSFX;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dashAttackBox = GetComponentInChildren<DashAttackBox>();
    }

    public override void OnEnterState()
    {
        TryDashAttack();
    }

    public override void OnExitState()
    {
    }

    public override void HandleState()
    {


    }

    public void TryDashAttack()
    {
        if (playerController.CanDashAttack)
            StartCoroutine(DashAttackCoroutine());
    }

    private IEnumerator DashAttackCoroutine()
    {
        playerController.CanDashAttack = false;

        // Anim
        playerController.AnimationController.SetTriggerForAnimations("Dash");

        // Sound
        AudioManager.Instance.PlaySFX(audioSource, dashAttackSFX[Random.Range(0, dashAttackSFX.Length)], 1);

        // VFX
        afterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());

        // Movement
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 dir = (mousePos - playerController.transform.position).normalized;
        playerController.PlayerMovementManager.TurnPlayer(dir);
        dir.y = Mathf.Clamp(dir.y, -0.5f, 0.5f);

        playerController.rb.linearVelocity += dir * dashModifier;

        // Damage Collider
        dashAttackBox.EnableCollider();

        yield return new WaitForSeconds(0.5f);

        if (afterImageCoroutine != null)
            StopCoroutine(afterImageCoroutine);

        playerController.rb.linearVelocity = Vector2.zero;
        dashAttackBox.DisableCollider();

        // Reset Direction
        playerController.PlayerMovementManager.TurnPlayer(playerController.PlayerMovementManager.currentInputDir);

        // Reset attack state
        playerController.ChangeState(PlayerStateEnum.Idle);

        yield return new WaitForSeconds(dashAttackDelay);
        playerController.CanDashAttack = true;
    }
}
