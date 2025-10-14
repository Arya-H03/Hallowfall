using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(CDetector))]
public class PlayerEnvironmentCheckHandler : MonoBehaviour
{
    private CDetector detector;

    [SerializeField] Transform groundCheckOrigin1;
    [SerializeField] Transform groundCheckOrigin2;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask interactionLayerMask;

    private PlayerController playerController;
    private ForestDetector forestDetector;

    [SerializeField] private int detectionRadius = 4;
    [SerializeField] Color vignetteColorInForest;

    

    public CDetector Detector { get => detector; }
    public LayerMask EnemyLayerMask { get => enemyLayer; }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        detector = GetComponent<CDetector>();
    }
    private void Start()
    {
        forestDetector = new ForestDetector(playerController, ZoneManager.Instance.GlobalTreeTilemap, detectionRadius, vignetteColorInForest);
    }
    private void FixedUpdate()
    {
        CheckForFloorType();
    }
    private void Update()
    {
        if (!playerController || playerController.IsDead) return;
        forestDetector.TryDetectForest();                                                         
    }
    private void CheckForFloorType()
    {
        RaycastHit2D rayCast = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);

        if (rayCast && !rayCast.collider.CompareTag(playerController.CurrentFloorType.ToString()))
        {
            //playerController.StateMachine.PlayerRunState.StopRunningSFX();
            switch (rayCast.collider.tag)
            {
                case "Ground":
                    playerController.CurrentFloorType = FloorTypeEnum.Ground;
                    break;
                case "Stone":
                    playerController.CurrentFloorType = FloorTypeEnum.Stone;
                    break;
                case "Grass":
                    playerController.CurrentFloorType = FloorTypeEnum.Grass;
                    break;
            }

            if (playerController.StateMachine.CurrentStateEnum == PlayerStateEnum.Run)
            {
                //playerController.StateMachine.PlayerRunState.StartRunningSFX();
            }

        }
    }
    //private void CheckForInteractions()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 1f, interactionLayerMask);
    //    if(hit) 
    //    {
    //        switch(hit.collider.tag)
    //        {
    //            case "Statue":
    //                if(currentInteractable == null)
    //                {
    //                    currentInteractable = hit.transform.gameObject.GetComponent<IInteractable>();
    //                    currentInteractable.OnIntercationBegin();
    //                }
    //                break;
    //            default:
    //                Debug.Log("Other Tag: " + hit.collider.tag);
    //                break;
    //        }
    //    }

    //    else
    //    {
    //        if(currentInteractable != null)
    //        {
    //            currentInteractable.OnIntercationEnd();
    //            currentInteractable = null;
    //        }
    //    }
    //}


}
