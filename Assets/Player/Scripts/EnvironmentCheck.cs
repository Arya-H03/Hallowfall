using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    [SerializeField] Transform headLevelCheckOrigin;
    [SerializeField] Transform midLevelCheckOrigin;
    [SerializeField] Transform groundCheckOrigin1;
    [SerializeField] Transform groundCheckOrigin2;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask interactionLayerMask;

    PlayerController playerController;

    private IInteractable currentInteractable;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!playerController.IsDead)
        {
            CheckForInteractions();
            CheckIfPlayerHasHitGround();
            CheckWhilePlayerIsFalling();
            RaycastHit2D headLevelCast = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.5f, layerMask);
            RaycastHit2D midLevelCast = Physics2D.Raycast(midLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.5f, layerMask);

            if (midLevelCast && !headLevelCast && playerController.CanHang)
            {

                playerController.ChangeState(PlayerStateEnum.Hang);
                playerController.PlayerHangingState.SetHaningPosition(midLevelCast);


            }




        }

    }

    private void CheckForInteractions()
    {
        RaycastHit2D hit = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 1f, interactionLayerMask);
        if(hit) 
        {
            switch(hit.collider.tag)
            {
                case "Statue":
                    if(currentInteractable == null)
                    {
                        currentInteractable = hit.transform.gameObject.GetComponent<IInteractable>();
                        currentInteractable.OnIntercationBegin();
                    }
                    break;
                default:
                    Debug.Log("Other Tag: " + hit.collider.tag);
                    break;
            }
        }

        else
        {
            if(currentInteractable != null)
            {
                currentInteractable.OnIntercationEnd();
                currentInteractable = null;
            }
        }
    }
    private void CheckWhilePlayerIsFalling()
    {
        RaycastHit2D[] rayCasts = new RaycastHit2D[2];
        rayCasts[0] = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);
        rayCasts[1] = Physics2D.Raycast(groundCheckOrigin2.transform.position, Vector2.down, 0.25f, groundLayer);

        //bool isGrounded = false;
        foreach (RaycastHit2D rayCast in rayCasts)
        {
            if (rayCast.collider != null)
            {
                //isGrounded = true;
                PlatformTag platformTag = rayCast.collider.gameObject.GetComponent<PlatformTag>();
                if (platformTag != playerController.CurrentPlatformElevation)
                {
                    playerController.CurrentPlatformElevation = platformTag;
                }

                if (rayCast.collider.CompareTag("Trap"))
                {
                    playerController.ChangeState(PlayerStateEnum.Death);
                    return;
                }
            }
        }

        //playerController.IsPlayerGrounded = isGrounded;

        if (!playerController.IsPlayerGrounded && !playerController.IsHanging)
        {
            if (playerController.CurrentStateEnum == PlayerStateEnum.Roll)
            {
                StartCoroutine(playerController.PlayerRollState.OnReachingLedgeWhileRolling(0.25f));
            }
            playerController.ChangeState(PlayerStateEnum.Fall);
        }
    }



    private void CheckIfPlayerHasHitGround()
    {
        if (playerController.rb.velocity.y <= 0 && playerController.IsFalling)
        {
            RaycastHit2D rayCast1 = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.3f, groundLayer);
            RaycastHit2D rayCast2 = Physics2D.Raycast(groundCheckOrigin2.transform.position, Vector2.down, 0.3f, groundLayer);
           
            if (rayCast1 ||rayCast2)
            {
                playerController.PlayerFallState.OnPlayerGrounded();             
                          
            }
            
        }
    }

}
