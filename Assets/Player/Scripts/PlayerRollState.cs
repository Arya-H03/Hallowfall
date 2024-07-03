using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    [SerializeField] float rollDuration = 0.5f; // Duration of the roll in seconds
    [SerializeField] float rollDistance = 1f;  // Distance to roll
    private float rollStartTime;

    private Vector3 currentPosition;
    private Vector3 targetPosition;

    public PlayerRollState()
    {
        this.stateEnum = PlayerStateEnum.Roll;
    }

    public override void OnEnterState()
    {
        currentPosition = playerController.transform.position;
        if (playerController.transform.localScale.x < 0)
        {
            targetPosition = currentPosition + Vector3.left * rollDistance;
        }
        else
        {
            targetPosition = currentPosition + Vector3.right * rollDistance;
        }

        rollStartTime = Time.time;


        playerController.IsRolling = true;
        playerController.PlayerCollision.BoxCollider2D.isTrigger = true;
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Kinematic;
        playerController.AnimationController.SetTriggerForAnimations("Roll");

        playerController.PlayerMovementManager.MoveSpeed = 0;
    }

    public override void OnExitState()
    {
        playerController.PlayerCollision.BoxCollider2D.isTrigger = false;
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;
        playerController.PlayerMovementManager.MoveSpeed = playerController.PlayerMovementManager.MoveSpeed;
        playerController.IsRolling = false;
    }

    public override void HandleState()
    {
        float elapsed = Time.time - rollStartTime;
        float t = elapsed / rollDuration;

        if (t < 1)
        {
            playerController.transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
        }
        
    }

    public void OnRollEnd()
    {
        if (playerController.PlayerMovementManager.currentDirection.x != 0)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
    }
}
