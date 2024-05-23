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

    [HideInInspector]
    public PlayerJump playerJump;
    [HideInInspector]
    public PlayerAttacks playerAttacks;
    [HideInInspector]
    public PlayerParry playerParry;
    [HideInInspector]
    public Rigidbody2D rb;
    public InputManager inputManager;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public ParticleSystem deathEffectParticle;
    [HideInInspector]
    public Player player;



    public bool canPlayerJump = true;
    public bool isPlayerGrounded = true;
    public bool isPlayerJumping = false;
    public bool canPlayerAttack = true;
    public bool isParrying  = false;
    public bool hasSword = false;
    public bool isDead = false;
    public bool isAttacking = false;


    private PlayerStateEnum currentStateEnum;
    private PlayerBaseState currentState;

    private PlayerIdleState playerIdleState;
    private PlayerRunState playerRunState;


    [SerializeField] PlayerFootSteps footSteps;

    #region Getters / Setters

    public PlayerAnimationController AnimationController { get => animationController; set => animationController = value; }
    public PlayerStateEnum CurrentStateEnum { get => currentStateEnum; set => currentStateEnum = value; }
    public PlayerBaseState CurrentState { get => currentState; set => currentState = value; }
    public PlayerIdleState PlayerIdleState { get => playerIdleState; set => playerIdleState = value; }
    public PlayerRunState PlayerRunState { get => playerRunState; set => playerRunState = value; }
    public PlayerMovementManager PlayerMovementManager { get => playerMovementManager; set => playerMovementManager = value; }

    #endregion
    private void Awake()
    {
        AnimationController = GetComponentInChildren<PlayerAnimationController>();

        PlayerMovementManager = GetComponent<PlayerMovementManager>();

        playerJump = GetComponentInChildren<PlayerJump>();
        playerAttacks = GetComponentInChildren<PlayerAttacks>();
        playerParry = GetComponent<PlayerParry>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEffectParticle = GetComponent<ParticleSystem>();
        player = GetComponent<Player>();

        
       
        PlayerIdleState = gameObject.AddComponent<PlayerIdleState>();
        PlayerIdleState.SetOnInitializeVariables(this);

        PlayerRunState = gameObject.AddComponent<PlayerRunState>();
        PlayerRunState.SetOnInitializeVariables(this,footSteps);

        CurrentStateEnum = PlayerStateEnum.Idle;
        CurrentState = PlayerIdleState;
    }
    public void OnMove(Vector2 dir)
    {
        PlayerMovementManager.HandleMovement(dir);
    }

    public void ChangeState(PlayerStateEnum stateEnum)
    {
        if (currentStateEnum != stateEnum /*&& canChangeState*/)
        {
            Debug.Log(CurrentStateEnum.ToString() + " to " + stateEnum.ToString());

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
                    CurrentState = PlayerRunState;
                    break;
            }

            CurrentStateEnum = stateEnum;
            CurrentState.OnEnterState();
        }

    }
    public void OnJumpStart()
    {
        if (isPlayerGrounded && canPlayerJump )
        {
            
            isPlayerJumping = true;
            isPlayerGrounded = false;
            canPlayerJump = false;
            canPlayerAttack = false;
            playerJump.StartJump();
        }     
    }

    public void OnStartAttack(int attackIndex)
    {
        if (hasSword)
        {
            playerAttacks.StartAttack(isPlayerJumping, attackIndex);
        }
        
    }

    public void OnParry()
    {
        if (hasSword)
        {
            
            playerParry.StartParry();
        }
       
  
    }

    public void HandelSwordEquipment(bool isEquiped)
    {
        animationController.ChangeAnimatorAC(isEquiped);
        hasSword = isEquiped;
    }

    public void RestoreHealth()
    {

    }


}
