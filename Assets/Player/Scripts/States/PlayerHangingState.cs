using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHangingState : PlayerBaseState
{

    public PlayerHangingState()
    {
        this.stateEnum = PlayerStateEnum.Hang;
    }
    public override void OnEnterState()
    {
        ChangePlayerInputActionsWhileHanging();
        playerController.AnimationController.SetBoolForAnimations("isHanging", true);
        playerController.IsHanging = true;  
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
        playerController.InputManager.InputActions.Guardian.Movement.performed -= playerController.InputManager.StartMove;
        playerController.InputManager.InputActions.Guardian.Movement.performed += JumpAway;
        playerController.InputManager.InputActions.Guardian.Jump.performed -= playerController.InputManager.Jump;
    }

    private void ResetPlayerInputActions()
    {
        playerController.InputManager.InputActions.Guardian.Movement.performed += playerController.InputManager.StartMove;
        playerController.InputManager.InputActions.Guardian.Movement.performed -= JumpAway;
        playerController.InputManager.InputActions.Guardian.Jump.performed += playerController.InputManager.Jump;     
    }

    private void JumpAway(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Dynamic;

        if(dir == Vector2.up)
        {
            playerController.PlayerCollision.Rb.velocity += new Vector2(0, 10);
        }

        else if(dir == Vector2.right)
        {
            if(playerController.gameObject.transform.localScale.x < 0)
            {
                playerController.PlayerCollision.Rb.velocity += new Vector2(5, 5);

            }
           
        }

        else if (dir == Vector2.left)
        {
            if (playerController.gameObject.transform.localScale.x > 0)
            {
                playerController.PlayerCollision.Rb.velocity += new Vector2(-5, 5);

            }
        }
        
        
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
