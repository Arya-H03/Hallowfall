using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;


public class PlayerDashState : PlayerState
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

    private Transform chargebar1;
    private Transform chargebar2;

    private DashAttackBox dashAttackBox;
    private AudioClip[] dashAttackSFX;

    private int maxDashCharges = 1;
    private int currentDashCharges = 1;

    private List<DashChargeSlot> availableDashCharges = new List<DashChargeSlot>();
    private List<DashChargeSlot> unAvailableDashCharges = new List<DashChargeSlot>();
    public float DashAttackDamage { get => dashAttackDamage; set => dashAttackDamage = value; }
    public List<DashChargeSlot> UnAvailableDashCharges { get => unAvailableDashCharges;}

    public PlayerDashState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum) : base(playerController, stateMachine, playerConfig, stateEnum)
    {
        this.stateEnum = PlayerStateEnum.Dash;

        dashAttackDamage = playerConfig.dashAttackDamage;
        dashModifier = playerConfig.dashModifier;
        dashAttackDelay = playerConfig.dashAttackDelay;
        dashAttackSFX = playerConfig.dashAttackSFX;
        dashDuration = playerConfig.dashduration;
        dashAttackBox = playerController.DashAttackBox;
        chargebar1 = UIManager.Instance.Chargebar1;
        chargebar2 = UIManager.Instance.Chargebar2;


        DashChargeSlot charge = new DashChargeSlot(dashAttackDelay, chargebar1);
        availableDashCharges.Add(charge);
    }
  
    public override void EnterState()
    {
        TryDashAttack();
    }

    private void TryDashAttack()
    {
        if (currentDashCharges > 0 && availableDashCharges.Count > 0)
        {
            playerController.CoroutineRunner.RunCoroutine(DashAttackCoroutine());
        }

    }
    public void HandleDashTimer()
    {
        if (currentDashCharges < maxDashCharges)
        {
            var copy = new List<DashChargeSlot>(UnAvailableDashCharges);
            foreach (var slot in copy)
            {
                if (slot.UpdateCharge(availableDashCharges, UnAvailableDashCharges)) currentDashCharges++;
            }
        }
    }

    private IEnumerator DashAttackCoroutine()
    {
        if (availableDashCharges.Count == 0) yield break;

        var charge = availableDashCharges[0];
        availableDashCharges.Remove(charge);
        UnAvailableDashCharges.Add(charge);
        currentDashCharges--;


        signalHub.OnAnimTrigger?.Invoke("Dash");
        signalHub.OnPlayRandomSFX?.Invoke(dashAttackSFX, 1);
        signalHub.OnAfterImageStart?.Invoke();
 
        Vector2 dashDirection = GetDashDirection();
        signalHub.OnApplyDirectionVelocity?.Invoke(dashDirection, dashModifier);
      
        dashAttackBox.EnableCollider();

        yield return new WaitForSeconds(dashDuration);

        signalHub.OnAfterImageStop?.Invoke();
        signalHub.OnResetVelocity?.Invoke();

        dashAttackBox.DisableCollider();

        signalHub.OnStateTransitionBasedOnMovement?.Invoke(PlayerStateEnum.Dash);

    }

    private Vector2 GetDashDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return (mousePos - playerController.transform.position).normalized;
    }

 
    public bool CanDashAttack() => currentDashCharges > 0 && availableDashCharges.Count > 0;

    public void CreateSecondDashCharge()
    {
        maxDashCharges = 2;
        currentDashCharges = maxDashCharges;
        chargebar2.parent.gameObject.SetActive(true);
        var charge2 = new DashChargeSlot(dashAttackDelay, chargebar2);
        availableDashCharges.Add(charge2);
    }
   
}
