using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputAction inputActions;

    [SerializeField] PlayerController player;
    [SerializeField] GameManager gameManager;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }

    public void OnEnable()
    {
        inputActions.Guardian.Movement.performed += StartMove;
        inputActions.Guardian.Movement.canceled += StopMove;

        inputActions.Guardian.Jump.performed += Jump;

        inputActions.Guardian.Attack1.performed += Attack1;
        inputActions.Guardian.Attack2.performed += Attack2;
        inputActions.Guardian.Attack3.performed += Attack3;

        inputActions.Guardian.Parry.performed += Parry;

        inputActions.Guardian.Pause.performed += Pause;   

        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Guardian.Movement.performed -= StartMove;
        inputActions.Guardian.Movement.canceled -= StopMove;

        inputActions.Guardian.Jump.performed -= Jump;

        inputActions.Guardian.Attack1.performed -= Attack1;
        inputActions.Guardian.Attack2.performed -= Attack2;
        inputActions.Guardian.Attack3.performed -= Attack3;
        inputActions.Guardian.Parry.performed -= Parry;

        inputActions.Guardian.Pause.performed -= Pause;

        inputActions.Disable();
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

    public void Attack1(InputAction.CallbackContext ctx)
    {
        player.OnStartAttack(1);
    }
    public void Attack2(InputAction.CallbackContext ctx)
    {
        player.OnStartAttack(2);
    }

    public void Attack3(InputAction.CallbackContext ctx)
    {
        player.OnStartAttack(3);
    }

    public void Parry(InputAction.CallbackContext ctx)
    {
        player.OnParry();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        gameManager.OnGamePause();
    }


}
