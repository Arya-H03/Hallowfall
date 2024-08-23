using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerRollState : PlayerBaseState
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip rollAC;

    [SerializeField] float rollDuration = 0.5f; // Duration of the roll in seconds
    [SerializeField] float rollDistance = 1f;  // Distance to roll

    [SerializeField] float rollCooldown = 1f;
    private float rollCooldownTimer = 0;
    private float rollStartTime;

    private Vector3 currentPosition;
    private Vector3 targetPosition;

    private bool isRollBlocked = false;
    public PlayerRollState()
    {
        this.stateEnum = PlayerStateEnum.Roll;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        playerController.AnimationController.SetTriggerForAnimations("Roll");
        AudioManager.Instance.PlaySFX(audioSource, rollAC);
        playerController.PlayerMovementManager.MoveSpeed = 0;
        rollCooldownTimer = 0;
        playerController.CanRoll = false;
    }

    public override void OnExitState()
    {
        isRollBlocked = false;
        playerController.PlayerMovementManager.MoveSpeed = playerController.PlayerMovementManager.MoveSpeed;
        playerController.IsRolling = false;
    }

    public override void HandleState()
    {
        CheckForRolling();
        if (!isRollBlocked)
        {
           
                float elapsed = Time.time - rollStartTime;
                float t = elapsed / rollDuration;

                if (t < 1)
                {
                    playerController.transform.position = Vector3.Lerp(currentPosition, targetPosition, t);
                }
            

        }
       

    }

    
    public void OnRollEnd()
    {
        if(playerController.CurrentStateEnum == PlayerStateEnum.Roll) 
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

    private void CheckForRolling()
    {       
       Vector3 rollDirection = playerController.transform.localScale.x < 0 ? Vector3.left : Vector3.right;      
       RaycastHit2D []hits = Physics2D.RaycastAll(transform.position, rollDirection, 0.25f);  
       foreach(var hit in hits)
        {
            if (hit.collider.CompareTag("BlockObject"))
            {

                isRollBlocked = true;
            }
            
        }
    }

    public void HandleRollCooldown()
    {
        if(rollCooldownTimer < rollCooldown)
        {
            rollCooldownTimer += Time.deltaTime;

            if(rollCooldownTimer >= rollCooldown)
            {
                playerController.CanRoll = true;
            }
        }
    }

    public IEnumerator OnReachingLedgeWhileRolling(float delay)
    {
        if (playerController.transform.localScale.x < 0)
        {
            playerController.PlayerCollision.Rb.velocity += new Vector2(-3,-1);
        }
        else
        {
            playerController.PlayerCollision.Rb.velocity += new Vector2(3, -1);
        }      
        playerController.PlayerCollision.BoxCollider2D.isTrigger = true;
        yield return new WaitForSeconds(delay);
        playerController.PlayerCollision.BoxCollider2D.isTrigger =false;
    }
}
