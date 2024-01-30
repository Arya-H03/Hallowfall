using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputAction inputActions;

    [SerializeField] PlayerController player;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }

    public void OnEnable()
    {
        inputActions.Shadow.Movement.performed += StartMove;
        inputActions.Shadow.Movement.canceled += StopMove;

        inputActions.Shadow.Jump.performed += Jump;

        inputActions.Shadow.Attack1.performed += Attack1;
        inputActions.Shadow.Attack2.performed += Attack2;
        inputActions.Shadow.Attack3.performed += Attack3;

        inputActions.Shadow.Parry.performed += Parry;

        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Shadow.Movement.performed -= StartMove;
        inputActions.Shadow.Movement.canceled -= StopMove;

        inputActions.Shadow.Jump.performed -= Jump;

        inputActions.Shadow.Attack1.performed -= Attack1;
        inputActions.Shadow.Attack2.performed -= Attack2;
        inputActions.Shadow.Attack3.performed -= Attack3;
        inputActions.Shadow.Parry.performed -= Parry;

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


}
