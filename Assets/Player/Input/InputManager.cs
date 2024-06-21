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

        inputActions.Guardian.Attack.performed += FirstSwing;
        
        inputActions.Guardian.SecondAttack.performed += DoubleSwing;

        inputActions.Guardian.Parry.performed += Parry;

        inputActions.Guardian.Pause.performed += Pause;   

        inputActions.Enable();
    }

    public void OnDisable()
    {
        inputActions.Guardian.Movement.performed -= StartMove;
        inputActions.Guardian.Movement.canceled -= StopMove;

        inputActions.Guardian.Jump.performed -= Jump;

        inputActions.Guardian.Attack.performed -= FirstSwing;
    
        inputActions.Guardian.Parry.performed -= Parry;

        inputActions.Guardian.Pause.performed -= Pause;

        inputActions.Guardian.SecondAttack.performed -= DoubleSwing;

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

    public void FirstSwing(InputAction.CallbackContext ctx)
    {
        player.OnSwordAttack();
    }
    //public void StabAttack(InputAction.CallbackContext ctx)
    //{
    //    player.OnSwordAttack(PlayerSwordAttackState.SwordAttackTypeEnum.Stab);
    //}

    //public void ChopAttack(InputAction.CallbackContext ctx)
    //{
    //    player.OnSwordAttack(PlayerSwordAttackState.SwordAttackTypeEnum.Chop);
    //}

    public void Parry(InputAction.CallbackContext ctx)
    {
        player.OnParry();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        gameManager.OnGamePause();
    }


    public void DoubleSwing(InputAction.CallbackContext ctx)
    {
        player.PlayerSwordAttackState.DoubleSwing();
    }


}
