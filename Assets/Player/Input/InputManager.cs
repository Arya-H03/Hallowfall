using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("InputManager");
                instance = go.AddComponent<InputManager>();
            }
            return instance;
        }
    }

    PlayerInputAction inputActions;

    [SerializeField] PlayerController player;

    public PlayerInputAction InputActions { get => inputActions; set => inputActions = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InputActions = new PlayerInputAction();
    }

    public void OnEnable()
    {
        InputActions.Guardian.Movement.performed += StartMove;
        InputActions.Guardian.Movement.canceled += StopMove;
        InputActions.Guardian.Interact.canceled += StopMove;

        InputActions.Guardian.Jump.performed += Jump;

        InputActions.Guardian.Attack.performed += FirstSwing;
        

        InputActions.Guardian.Roll.performed += Roll;

        InputActions.Guardian.Parry.performed += StartParry;


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

        InputActions.Guardian.Parry.performed -= StartParry;

        InputActions.Guardian.Pause.performed -= Pause;


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
        GameManager.Instance.OnGamePause();
    }

    public void StartParry(InputAction.CallbackContext ctx)
    {
        player.OnStartParry();
    }

}
