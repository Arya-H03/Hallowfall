using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputAction inputActions;

    [SerializeField] PlayerController player;
    [SerializeField] GameManager gameManager;

    public PlayerInputAction InputActions { get => inputActions; set => inputActions = value; }

    private void Awake()
    {
        InputActions = new PlayerInputAction();
    }

    public void OnEnable()
    {
        InputActions.Guardian.Movement.performed += StartMove;
        InputActions.Guardian.Movement.canceled += StopMove;

        InputActions.Guardian.Jump.performed += Jump;

        InputActions.Guardian.Attack.performed += FirstSwing;
        
        InputActions.Guardian.SecondAttack.performed += DoubleSwing;

        InputActions.Guardian.Roll.performed += Roll;

        InputActions.Guardian.Parry.performed += Parry;

        InputActions.Guardian.Pause.performed += Pause;   

        InputActions.Enable();
    }

    public void OnDisable()
    {
        InputActions.Guardian.Movement.performed -= StartMove;
        InputActions.Guardian.Movement.canceled -= StopMove;

        InputActions.Guardian.Jump.performed -= Jump;

        InputActions.Guardian.Attack.performed -= FirstSwing;
    
        InputActions.Guardian.Roll.performed -= Roll;

        InputActions.Guardian.Parry.performed -= Parry;

        InputActions.Guardian.Pause.performed -= Pause;

        InputActions.Guardian.SecondAttack.performed -= DoubleSwing;

        InputActions.Disable();
    }

    public void StartMove(InputAction.CallbackContext ctx)
    {
        player.OnMove(ctx.ReadValue<Vector2>());
    }

    public void StopMove(InputAction.CallbackContext ctx)
    {
        player.OnMove(Vector2.zero);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        player.OnJumpStart();
    }

    public void FirstSwing(InputAction.CallbackContext ctx)
    {
        player.OnSwordAttack();
    }
   
    public void Roll(InputAction.CallbackContext ctx)
    {
        player.OnRoll();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        gameManager.OnGamePause();
    }


    public void DoubleSwing(InputAction.CallbackContext ctx)
    {
        player.PlayerSwordAttackState.DoubleSwing();
    }

    public void Parry(InputAction.CallbackContext ctx)
    {
        player.OnParry();
    }


}
