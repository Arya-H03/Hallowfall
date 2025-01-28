using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHangingState : PlayerBaseState
{
    private AudioSource audioSource;
   [SerializeField] private AudioClip hangingAC;
   [SerializeField] private AudioClip jumpingAC;

    
    public PlayerHangingState()
    {
        this.stateEnum = PlayerStateEnum.Hang;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override void OnEnterState()
    {
        ChangePlayerInputActionsWhileHanging();
        
        playerController.AnimationController.SetBoolForAnimations("isHanging", true);
        
        playerController.IsHanging = true;  
        playerController.CanPlayerJump= true;  
        AudioManager.Instance.PlaySFX(audioSource,hangingAC);
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;
        
        
    }

    public override void OnExitState()
    {
        playerController.AnimationController.SetBoolForAnimations("isHanging", false);
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;
        playerController.IsHanging = false;
        ResetPlayerInputActions();
    }

    public override void HandleState()
    {


    }

    private void ChangePlayerInputActionsWhileHanging()
    {
        InputManager.Instance.InputActions.Guardian.Movement.performed -= InputManager.Instance.StartMove;
        InputManager.Instance.InputActions.Guardian.Movement.performed += JumpAway;
        InputManager.Instance.InputActions.Guardian.Jump.performed -= InputManager.Instance.Jump;
    }

    private void ResetPlayerInputActions()
    {
        InputManager.Instance.InputActions.Guardian.Movement.performed += InputManager.Instance.StartMove;
        InputManager.Instance.InputActions.Guardian.Movement.performed -= JumpAway;
        InputManager.Instance.InputActions.Guardian.Jump.performed += InputManager.Instance.Jump;     
    }

    private void JumpAway(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;

        if(dir == Vector2.up)
        {
            playerController.PlayerCollision.Rb.velocity += new Vector2(0, 10);
            playerController.AnimationController.SetTriggerForAnimations("HangingPushUp");
        }

        else if(dir == Vector2.right)
        {
            if(playerController.gameObject.transform.localScale.x < 0)
            {
                playerController.PlayerCollision.Rb.velocity += new Vector2(5, 5);
                playerController.AnimationController.SetTriggerForAnimations("HangingPushUp");

            }
           
        }

        else if (dir == Vector2.left)
        {
            if (playerController.gameObject.transform.localScale.x > 0)
            {
                playerController.PlayerCollision.Rb.velocity += new Vector2(-5, 5);
                playerController.AnimationController.SetTriggerForAnimations("HangingPushUp");

            }
        }

        AudioManager.Instance.PlaySFX(audioSource, jumpingAC);


        StartCoroutine(HandleHangingCooldown());

        playerController.ChangeState(PlayerStateEnum.Idle);
    }
    private IEnumerator HandleHangingCooldown()
    {
        playerController.CanHang = false;
        yield return new WaitForSeconds(0.35f);
        playerController.CanHang = true;
    }

    public void SetHaningPosition(RaycastHit2D hitPoint)
    {
        Vector3 playerPosition = playerController.gameObject.transform.position;
        Vector3 rightLedge = hitPoint.collider.bounds.max;
        Vector3 leftLedge = new Vector3(rightLedge.x - (2 * hitPoint.collider.bounds.extents.x), rightLedge.y, rightLedge.z);

       if (Vector3.Distance(playerPosition,rightLedge) > Vector3.Distance(playerPosition, leftLedge))
       {
            playerController.gameObject.transform.position = new Vector3(leftLedge.x - 0.4f, leftLedge.y - 0.55f, transform.position.z);
       }
        else
        {
            playerController.gameObject.transform.position = new Vector3(rightLedge.x + 0.4f, rightLedge.y - 0.55f, transform.position.z);
        }
       
    }
}
