using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class PlayerController : MonoBehaviour
{

    private PlayerAnimationController animationController;
    private PlayerMovementManager playerMovementManager;
    [SerializeField] private GameManager gameManager;

    [HideInInspector]
    public PlayerAttacks playerAttacks;
    [HideInInspector]
    public Rigidbody2D rb;
    [SerializeField] private InputManager inputManager;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public ParticleSystem deathEffectParticle;

    private Player player;
    private PlayerCollision playerCollision;



    [SerializeField] private bool canPlayerJump = true;
    [SerializeField] private bool isPlayerGrounded = true;
    [SerializeField] private bool isPlayerJumping = false;
    [SerializeField] private bool canPlayerAttack = true;
    [SerializeField] private bool isParrying  = false;
    [SerializeField] private bool hasSword = true;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private bool isRolling = false;
    [SerializeField] private bool isHanging = false;
    [SerializeField] private bool canRoll = true;
    [SerializeField] private bool canHang = true;
    [SerializeField] private bool isFalling = false;

    [SerializeField] private bool isFacingWall = false;
    [SerializeField] private bool isFacingLedge = false;


    [SerializeField] private PlayerStateEnum currentStateEnum;
    private PlayerBaseState currentState;

    private PlayerIdleState playerIdleState;
    private PlayerRunState playerRunState;
    private PlayerJumpState playerJumpState;
    private PlayerSwordAttackState playerSwordAttackState;
    private PlayerParryState playerParryState;
    private PlayerRollState playerRollState;
    private PlayerHangingState playerHangingState;
    private PlayerFallState playerFallState;


    private PlayerFootSteps footSteps;

    #region Getters / Setters

    public PlayerAnimationController AnimationController { get => animationController; set => animationController = value; }
    public PlayerStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public PlayerIdleState PlayerIdleState { get => playerIdleState; set => playerIdleState = value; }
    public PlayerRunState PlayerRunState { get => playerRunState; set => playerRunState = value; }
    public PlayerMovementManager PlayerMovementManager { get => playerMovementManager; set => playerMovementManager = value; }
    public GameManager GameManager { get => gameManager;}
    public PlayerJumpState PlayerJumpState { get => playerJumpState; set => playerJumpState = value; }
    public bool CanPlayerJump { get => canPlayerJump; set => canPlayerJump = value; }
    public bool IsPlayerGrounded { get => isPlayerGrounded; set => isPlayerGrounded = value; }
    public bool IsPlayerJumping { get => isPlayerJumping; set => isPlayerJumping = value; }
    public bool CanPlayerAttack { get => canPlayerAttack; set => canPlayerAttack = value; }
    public bool IsParrying { get => isParrying; set => isParrying = value; }
    public bool HasSword { get => hasSword; set => hasSword = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public PlayerSwordAttackState PlayerSwordAttackState { get => playerSwordAttackState; set => playerSwordAttackState = value; }
    public PlayerParryState PlayerParryState { get => playerParryState; set => playerParryState = value; }
    public Player Player { get => player; set => player = value; }
    public PlayerFootSteps FootSteps { get => footSteps; set => footSteps = value; }
    public PlayerRollState PlayerRollState { get => playerRollState; set => playerRollState = value; }
    public PlayerCollision PlayerCollision { get => playerCollision; set => playerCollision = value; }
    public bool IsRolling { get => isRolling; set => isRolling = value; }
    public bool CanRoll { get => canRoll; set => canRoll = value; }
    public PlayerHangingState PlayerHangingState { get => playerHangingState; set => playerHangingState = value; }
    public InputManager InputManager { get => inputManager; set => inputManager = value; }
    public bool CanHang { get => canHang; set => canHang = value; }
    public bool IsHanging { get => isHanging; set => isHanging = value; }
    public bool IsFacingWall { get => isFacingWall; set => isFacingWall = value; }
    public bool IsFacingLedge { get => isFacingLedge; set => isFacingLedge = value; }
    public PlayerFallState PlayerFallState { get => playerFallState; set => playerFallState = value; }
    public bool IsFalling { get => isFalling; set => isFalling = value; }

    #endregion
    private void Awake()
    {
        
        AnimationController = GetComponentInChildren<PlayerAnimationController>();

        PlayerMovementManager = GetComponent<PlayerMovementManager>();

        PlayerCollision = GetComponent<PlayerCollision>();

        playerAttacks = GetComponentInChildren<PlayerAttacks>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEffectParticle = GetComponent<ParticleSystem>();
        Player = GetComponent<Player>();
        footSteps = GetComponentInChildren<PlayerFootSteps>();  

        
       
        PlayerIdleState = GetComponentInChildren<PlayerIdleState>();
        PlayerIdleState.SetOnInitializeVariables(this);

        PlayerRunState = GetComponentInChildren<PlayerRunState>();
        PlayerRunState.SetOnInitializeVariables(this,footSteps);

        PlayerJumpState = GetComponentInChildren<PlayerJumpState>();
        PlayerJumpState.SetOnInitializeVariables(this);

        PlayerSwordAttackState = GetComponentInChildren<PlayerSwordAttackState>();
        PlayerSwordAttackState.SetOnInitializeVariables(this);

        PlayerParryState = GetComponentInChildren<PlayerParryState>();
        PlayerParryState.SetOnInitializeVariables(this);

        PlayerRollState = GetComponentInChildren<PlayerRollState>();    
        PlayerRollState.SetOnInitializeVariables(this);

        PlayerHangingState = GetComponentInChildren<PlayerHangingState>();
        PlayerHangingState.SetOnInitializeVariables(this);

        PlayerFallState = GetComponentInChildren<PlayerFallState>(); 
        PlayerFallState.SetOnInitializeVariables(this);


        CurrentStateEnum = PlayerStateEnum.Idle;
        CurrentState = PlayerIdleState;
    }

    private void Update()
    {
        currentState.HandleState();
        PlayerRollState.HandleRollCooldown();
    }
    public void ChangeState(PlayerStateEnum stateEnum)
    {
        if (currentStateEnum != stateEnum /*&& canChangeState*/)
        {
            //Debug.Log(CurrentStateEnum.ToString() + " to " + stateEnum.ToString());

            if (CurrentState != null)
            {
                CurrentState.OnExitState();
            }

            switch (stateEnum)
            {

                case PlayerStateEnum.Idle:
                    CurrentState = PlayerIdleState;
                    break;
                case PlayerStateEnum.Run:
                    if(!IsAttacking && !IsParrying && !IsPlayerJumping)
                    {
                        CurrentState = PlayerRunState;
                    }
                    
                    break;
                case PlayerStateEnum.Jump:
                    CurrentState = PlayerJumpState;
                    break;
                case PlayerStateEnum.SwordAttack:
                    CurrentState = PlayerSwordAttackState;
                    break;
                 case PlayerStateEnum.Parry:
                    CurrentState = PlayerParryState;
                    break;
                case PlayerStateEnum.Roll:
                    CurrentState = PlayerRollState;
                    break;
                 case PlayerStateEnum.Hang:
                    CurrentState = PlayerHangingState;
                    break;
                case PlayerStateEnum.Fall:
                    CurrentState = PlayerFallState;
                    break;
            }

            CurrentStateEnum = stateEnum;
            CurrentState.OnEnterState();
        }

    }

    public void OnMove(Vector2 dir)
    {
        PlayerMovementManager.HandleMovement(dir);
    }


    public void OnJumpStart()
    {
        if (IsPlayerGrounded && CanPlayerJump && !IsPlayerJumping)
        {
            ChangeState(PlayerStateEnum.Jump);
        }     
    }

    public void OnSwordAttack()
    {
        if (HasSword && !IsAttacking && CanPlayerAttack)
        {
    
            ChangeState(PlayerStateEnum.SwordAttack);
        }
        
    }

    public void OnParry()
    {
        if(isPlayerGrounded && !isRolling)
        {
            ChangeState(PlayerStateEnum.Parry);
        }
        
    }
    public void OnRoll()
    {
        if(IsPlayerGrounded && !isPlayerJumping && !isRolling && CanRoll)
        {
            ChangeState(PlayerStateEnum.Roll);
        }
        
    }

    public void HandelSwordEquipment(bool isEquiped)
    {
        animationController.ChangeAnimatorAC(isEquiped);
        HasSword = isEquiped;
    }

    public void RestoreHealth()
    {

    }

    //public void OnPlayerHang(Transform hagningPoint)
    //{
    //    animationController.SetBoolForAnimations("isHanging", true);
    //    PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;
    //    transform.position = hagningPoint.position;
    //    inputManager.DisablePlayerMovement();
    //    isPlayerJumping = false;
    //    CanPlayerJump = true;
    //    isPlayerGrounded = true;

    //}
    
}
