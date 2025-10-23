using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour, IInitializeable<PlayerController>, IMoveable
{
    private PlayerController playerController;
    private ZoneManager zoneManager;
    private FlowFieldManager flowFieldManager;
    private PlayerSignalHub signalHub;

    private float flowFieldGenerationDelay = 0.3f;
    private float flowFieldGenerationTimer = 0.3f;

    private Vector2 currentInputDir;
    private float speedModifer = 1;

    private float moveSpeed;
    private bool canMove = false;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float SpeedModifer { get => speedModifer; }
    public Rigidbody2D Rb { get; set; }
    public Vector2 FaceDirection { get; set; }


    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        signalHub = playerController.PlayerSignalHub;
        zoneManager = ZoneManager.Instance;
        flowFieldManager = FlowFieldManager.Instance;
        Rb = playerController.Rb;

        signalHub.OnChangeMoveSpeed += ChangeMoveSpeed;
        signalHub.OnAllowMovement += AllowPlayerToMove;
        signalHub.OnApplyForwardVelocity += ApplyForwardVelocity;
        signalHub.OnApplyDirectionVelocity += ApplyDirectionVelocity;
        signalHub.OnResetVelocity += ResetVelocity;
        signalHub.OnStateTransitionBasedOnMovement += ManageSateTransitionBasedOnMovementInput;
        signalHub.OnTurning += TryToTurn;
        signalHub.OnTurningToMousePos += TurnPlayerWithMousePos;
        signalHub.OnChangeSpeedModifier += ChangeModifySpeed;
        signalHub.RequestInputDir += GetCurrentInputDir;
        signalHub.FacingDirctionBinding = new PropertyBinding<Vector2>
            (
                () => FaceDirection,
                (value) => FaceDirection = value
            );
    }

    private void OnDisable()
    {
        signalHub.OnChangeMoveSpeed -= ChangeMoveSpeed;
        signalHub.OnAllowMovement -= AllowPlayerToMove;
        signalHub.OnApplyForwardVelocity -= ApplyForwardVelocity;
        signalHub.OnApplyDirectionVelocity -= ApplyDirectionVelocity;
        signalHub.OnResetVelocity -= ResetVelocity;
        signalHub.OnStateTransitionBasedOnMovement -= ManageSateTransitionBasedOnMovementInput;
        signalHub.OnTurning += TryToTurn;
        signalHub.OnTurningToMousePos -= TurnPlayerWithMousePos;
        signalHub.OnChangeSpeedModifier -= ChangeModifySpeed;
        signalHub.RequestInputDir -= GetCurrentInputDir;
    }
    private void Update()
    {
        //HandleUpdatingGlobalFlowField();
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Move(currentInputDir, moveSpeed);
        }
    }
    public void Move(Vector2 velocity, float speed)
    {
        Vector2 targetPos = Rb.position + (speed * speedModifer * Time.deltaTime * currentInputDir);
        Vector2 smoothPos = Vector2.Lerp(Rb.position, targetPos, 1f);
        Rb.MovePosition(smoothPos);
    }

    public void StopMove() { }
    public void TryToTurn(Vector2 direction)
    {
        float scaleX = 1;

        if (direction.x > 0)
        {
            scaleX = 1;
        }
        else if (direction.x < 0)
        {
            scaleX = -1;

        }
        else if (direction.x == 0)
        {
            scaleX = transform.localScale.x;
        }

        FaceDirection = new Vector2(scaleX, 0);
        transform.localScale = new Vector3(scaleX, 1, 1);

    }

    private void ManageTransitionRunState()
    {
        if (!playerController.IsParrying && !playerController.IsAttacking && !playerController.IsRolling)
        {
            if (currentInputDir != Vector2.zero)
            {

                signalHub.OnChangeState?.Invoke(PlayerStateEnum.Run);
            }
            else
            {
                signalHub.OnChangeState?.Invoke(PlayerStateEnum.Idle);
            }
        }

    }
    public void SetMovementInputDirection(Vector2 dir)
    {
        currentInputDir = dir;
        TryToTurn(currentInputDir);
        ManageTransitionRunState();
    }

    private void TurnPlayerWithMousePos()
    {
        Vector3 mousePos = MyUtils.GetMousePos();

        if (mousePos.x - transform.position.x > 0f)
        {
            TryToTurn(Vector2.right);
        }

        if (mousePos.x - transform.position.x < 0f)
        {
            TryToTurn(Vector2.left);
        }
    }
    private void HandleUpdatingGlobalFlowField()
    {
        if (zoneManager && flowFieldGenerationTimer >= flowFieldGenerationDelay)
        {
            flowFieldManager.UpdateGlobalFlowField(playerController.GetPlayerPos()) ;
            flowFieldGenerationTimer = 0;

        }
        flowFieldGenerationTimer += Time.deltaTime;
    }

    private void ChangeMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void AllowPlayerToMove(bool value)
    {
        canMove = value;
    }

    private void ApplyDirectionVelocity(Vector2 dir, float modifier)
    {
        TryToTurn(dir);
        playerController.Rb.linearVelocity += dir.normalized * modifier;
    }
    private void ApplyForwardVelocity(float modifier)
    {
        TryToTurn(currentInputDir);
        playerController.Rb.linearVelocity += FaceDirection.normalized * modifier;
    }
    private void ResetVelocity()
    {
        playerController.Rb.linearVelocity = Vector2.zero;
    }
    private void ManageSateTransitionBasedOnMovementInput(PlayerStateEnum currentStateEnum)
    {
        if (playerController.StateMachine.CurrentStateEnum == currentStateEnum)
        {
            if (currentInputDir != Vector2.zero)
            {
                signalHub.OnChangeState?.Invoke(PlayerStateEnum.Run);
            }
            else
            {
                signalHub.OnChangeState?.Invoke(PlayerStateEnum.Idle);
            }
        }
    }
    private void ChangeModifySpeed(float modifir)
    {
        speedModifer = modifir;
    }

    public Vector2 GetCurrentInputDir() { return currentInputDir; }
}
