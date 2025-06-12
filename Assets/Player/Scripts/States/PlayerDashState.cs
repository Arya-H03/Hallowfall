using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;


public class PlayerDashState : PlayerBaseState
{
    [System.Serializable]
    public class DashChargeSlot
    {
        private float delayTimer;
        private float dashDelay;
        private Transform dashChargeBar;

        public DashChargeSlot(float dashDelay, Transform dashChargeBar)
        {
            this.dashDelay = dashDelay;
            this.dashChargeBar = dashChargeBar;
        }
        public float DelayTimer { get => delayTimer; set => delayTimer = value; }

        public bool UpdateCharge(List<DashChargeSlot> availableDashCharges, List<DashChargeSlot> unavailableDashCharges)
        {
            DelayTimer += Time.deltaTime;

            float scale = DelayTimer / dashDelay;
            dashChargeBar.localScale = new Vector3(Mathf.Clamp01(scale), 1, 1);

            if (DelayTimer >= dashDelay)
            {
               
                DelayTimer = 0;
                availableDashCharges.Add(this);
                unavailableDashCharges.Remove(this);
                return true;
            }
            return false;
        }
    }

    private float dashAttackDamage;
    private float dashModifier;
    private float dashDuration;
    private float dashAttackDelay;

    private DashAttackBox dashAttackBox;
    private AudioClip[] dashAttackSFX;
    private Coroutine afterImageCoroutine;
    private AudioSource audioSource;

    private int maxDashCharges = 1;
    private int currentDashCharges = 1;

    [SerializeField] private Transform chargebar1;
    [SerializeField] private Transform chargebar2;

    public List<DashChargeSlot> availableDashCharges = new List<DashChargeSlot>();
    public List<DashChargeSlot> unAvailableDashCharges = new List<DashChargeSlot>();
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
        dashDuration = config.dashduration;
    }
    private void OnEnable()
    {
        SkillEvents.OnDoubleDashSkillUnlocked += DoubleDashSkillLogic;
        SkillEvents.OnPerfectTimingSkillUnlocked += PerfectTimingSkillLogic;
    }
    private void OnDisable()
    {
        SkillEvents.OnDoubleDashSkillUnlocked -= DoubleDashSkillLogic;
        SkillEvents.OnPerfectTimingSkillUnlocked -= PerfectTimingSkillLogic;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dashAttackBox = GetComponentInChildren<DashAttackBox>();
    }

    public override void Start()
    {
        base.Start();
        DashChargeSlot charge1 = new DashChargeSlot(dashAttackDelay, chargebar1);
        availableDashCharges.Add(charge1);
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

    public void HandleDashTimer()
    {
        if (currentDashCharges < maxDashCharges)
        {
            var copy = new List<DashChargeSlot>(unAvailableDashCharges);
            foreach (var slot in copy)
            {
                if (slot.UpdateCharge(availableDashCharges, unAvailableDashCharges)) currentDashCharges++;
            }
        }
    }


    public void TryDashAttack()
    {
        if (currentDashCharges > 0 && availableDashCharges.Count > 0)
            StartCoroutine(DashAttackCoroutine());
    }

    private IEnumerator DashAttackCoroutine()
    {
        if (availableDashCharges.Count == 0) yield break;

        var charge = availableDashCharges[availableDashCharges.Count-1];
        availableDashCharges.Remove(charge);
        unAvailableDashCharges.Add(charge);
        currentDashCharges--;

        TriggerDashEffects();

        Vector2 dashDirection = GetDashDirection();
        playerController.PlayerMovementManager.TurnPlayer(dashDirection);
        dashDirection.y = Mathf.Clamp(dashDirection.y, -0.5f, 0.5f);
        playerController.rb.linearVelocity += dashDirection * dashModifier;

        dashAttackBox.EnableCollider();

        yield return new WaitForSeconds(dashDuration);

        if (afterImageCoroutine != null) StopCoroutine(afterImageCoroutine);
        playerController.rb.linearVelocity = Vector2.zero;
        dashAttackBox.DisableCollider();
        playerController.PlayerMovementManager.TurnPlayer(playerController.PlayerMovementManager.currentInputDir);
        playerController.ChangeState(PlayerStateEnum.Idle);
    }

    private void TriggerDashEffects()
    {
        playerController.AnimationController.SetTriggerForAnimations("Dash");
        AudioManager.Instance.PlaySFX(audioSource, dashAttackSFX[Random.Range(0, dashAttackSFX.Length)], dashAttackSFX.Length);
        afterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
    }

    private Vector2 GetDashDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return (mousePos - playerController.transform.position).normalized;
    }

    private void DoubleDashSkillLogic()
    {
        maxDashCharges = 2;
        currentDashCharges = maxDashCharges;
        chargebar2.parent.gameObject.SetActive(true);
        var charge2 = new DashChargeSlot(dashAttackDelay, chargebar2);
        availableDashCharges.Add(charge2);
    }

    public bool CanDashAttack() => currentDashCharges > 0 && availableDashCharges.Count > 0;

    private void PerfectTimingSkillLogic()
    {
        PlayerParryState.OnParrySuccessful += () =>
        {
            foreach (DashChargeSlot charge in unAvailableDashCharges)
            {
                charge.DelayTimer += 1;
            }
        };

    }


}
