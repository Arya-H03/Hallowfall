using System;
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
    [SerializeField] LayerMask enemyLayer;
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
            
            CheckWhilePlayerIsFalling();
            CheckForFloorType();




            RaycastHit2D headLevelCast = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.5f, layerMask);
            RaycastHit2D midLevelCast = Physics2D.Raycast(midLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.5f, layerMask);

            if (midLevelCast && !headLevelCast && playerController.CanHang)
            {
                Debug.Log("Hang");
                playerController.ChangeState(PlayerStateEnum.Hang);
                playerController.PlayerHangingState.SetHaningPosition(midLevelCast);


            }


            if (!playerController.IsPlayerGrounded && !playerController.IsHanging && !playerController.IsAttacking )
            {
                if (playerController.CurrentStateEnum == PlayerStateEnum.Roll)
                {
                    StartCoroutine(playerController.PlayerRollState.OnReachingLedgeWhileRolling(0.25f));
                }
                
            }

        }

    }
    private void CheckForFloorType()
    {
        RaycastHit2D rayCast = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);
        if (rayCast && rayCast.collider.tag != playerController.CurrentFloorType.ToString()) 
        {
            playerController.PlayerRunState.StopRunningSFX();
            switch (rayCast.collider.tag)
            {
                case "Ground":
                    playerController.CurrentFloorType = FloorTypeEnum.Ground;
                    break;
                case "Wood":
                    playerController.CurrentFloorType = FloorTypeEnum.Wood;
                    break;
                case "Grass":
                    playerController.CurrentFloorType = FloorTypeEnum.Grass;
                    break;
            }

            if (playerController.CurrentStateEnum == PlayerStateEnum.Run)
            {
                playerController.PlayerRunState.StartRunningSFX();
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

        

        if (rayCasts[0])
        {
            HandleCastResultForGroundChecking(rayCasts[0]);
        }
        else HandleCastResultForGroundChecking(rayCasts[1]);

        //if (!rayCasts[1] && !rayCasts[0] && !playerController.IsAttacking &&!playerController.IsHanging)
        //{
        //    playerController.IsPlayerGrounded = false;
        //    playerController.ChangeState(PlayerStateEnum.Fall);
        //} 

       
    }

    private void HandleCastResultForGroundChecking(RaycastHit2D rayCast)
    {
        if (rayCast.collider != null)
        {
            if (rayCast.collider.CompareTag("Trap"))
            {
                playerController.ChangeState(PlayerStateEnum.Death);
                return;
            }

            //else if (playerController.CurrentStateEnum == PlayerStateEnum.Fall)
            //{
            //    playerController.PlayerFallState.OnPlayerGrounded();
            //}

        }
    }


}
