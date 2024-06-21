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
    public InputManager inputManager;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public ParticleSystem deathEffectParticle;

    private Player player;



    [SerializeField] private bool canPlayerJump = true;
    [SerializeField] private bool isPlayerGrounded = true;
    [SerializeField] private bool isPlayerJumping = false;
    [SerializeField] private bool canPlayerAttack = true;
    [SerializeField] private bool isParrying  = false;
    [SerializeField] private bool hasSword = true;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isAttacking = false;


    private PlayerStateEnum currentStateEnum;
    private PlayerBaseState currentState;

    private PlayerIdleState playerIdleState;
    private PlayerRunState playerRunState;
    private PlayerJumpState playerJumpState;
    private PlayerSwordAttackState playerSwordAttackState;
    private PlayerParryState playerParryState;


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

    #endregion
    private void Awake()
    {
        
        AnimationController = GetComponentInChildren<PlayerAnimationController>();

        PlayerMovementManager = GetComponent<PlayerMovementManager>();

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


        CurrentStateEnum = PlayerStateEnum.Idle;
        CurrentState = PlayerIdleState;
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
        if (HasSword && !IsAttacking)
        {
    
            ChangeState(PlayerStateEnum.SwordAttack);
        }
        
    }

    public void OnParry()
    {
        if (HasSword)
        {
            ChangeState(PlayerStateEnum.Parry);
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

    
}
