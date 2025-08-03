using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour,IInitializeable<PlayerController>
{
    private static PlayerInputHandler instance;

    public static PlayerInputHandler Instance
    {
        get
        {         
            return instance;
        }
    }

    private PlayerInputAction inputActions;

    private PlayerController playerController;

    public PlayerInputAction InputActions { get => inputActions; }

    public void Init( PlayerController playerController )
    {
        this.playerController = playerController;
        

    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnEnable()
    {
        inputActions = new PlayerInputAction();

        InputActions.Guardian.Movement.performed += StartMove;
        InputActions.Guardian.Movement.canceled += StopMove;
        InputActions.Guardian.Interact.canceled += StopMove;
        InputActions.Guardian.Swing.performed += Attack;
        InputActions.Guardian.DashAttack.performed += DashAttack;
        InputActions.Guardian.Roll.performed += Roll;
        InputActions.Guardian.Parry.performed += StartParry;
        InputActions.Guardian.Pause.performed += Pause;
        InputActions.Enable();
    }

    public void OnDisable()
    {
        InputActions.Guardian.Movement.performed -= StartMove;
        InputActions.Guardian.Movement.canceled -= StopMove;

        InputActions.Guardian.Swing.performed -= Attack;

        InputActions.Guardian.DashAttack.performed -= DashAttack;

        InputActions.Guardian.Roll.performed -= Roll;

        InputActions.Guardian.Parry.performed -= StartParry;

        InputActions.Guardian.Pause.performed -= Pause;


        InputActions.Disable();
    }

    public void StartMove(InputAction.CallbackContext ctx)
    {
        playerController.OnMoveInput(ctx.ReadValue<Vector2>());
    }

    public void StopMove(InputAction.CallbackContext ctx)
    {
        playerController.OnMoveInput(Vector2.zero);
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        playerController.OnSwordAttackInput();
    }
    public void Roll(InputAction.CallbackContext ctx)
    {
        playerController.OnRollInput();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        UIManager.Instance.OnGamePause();
    }

    public void StartParry(InputAction.CallbackContext ctx)
    {
        playerController.OnParryInput();
    }

    public void DashAttack(InputAction.CallbackContext ctx)
    {
        playerController.OnDashInput();

        
    }
}
