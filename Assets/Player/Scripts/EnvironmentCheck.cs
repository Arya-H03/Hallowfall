using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnvironmentCheck : MonoBehaviour
{
    [SerializeField] Transform headLevelCheckOrigin;
    [SerializeField] Transform midLevelCheckOrigin;

    [SerializeField] LayerMask layerMask;

    PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {

        RaycastHit2D headLevelCast = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.4f, layerMask);
        //if (headLevelCast)
        //{
        //    Debug.Log(" head ");
        //}

        //Debug.DrawRay(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 0.25f, 0), Color.red);
        RaycastHit2D midLevelCast = Physics2D.Raycast(midLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 0.4f, layerMask);
        //if (midLevelCast)
        //{
        //    Debug.Log(" mid ");
        //}
        //Debug.DrawRay(midLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 0.25f, 0), Color.red);

        if (midLevelCast && !headLevelCast && playerController.CanHang) {
            
            playerController.ChangeState(PlayerStateEnum.Hang);
            playerController.PlayerHangingState.SetHaningPosition(midLevelCast);
            

        }
    }

   
}
