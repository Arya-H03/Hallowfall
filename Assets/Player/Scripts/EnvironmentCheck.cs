using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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

    private bool isInForest = false;
    private readonly int forestCheckRadius = 4;

   
    private Tilemap lastTreeTilemap;

    private HashSet<Vector3Int> fadedCells = new();
    private Dictionary<Vector3Int,Coroutine> fadeCoroutinesDict = new();
  

    private void Update()
    {
        
        if (playerController && !playerController.IsDead)
        {

            CheckIfPlayerOnTreeTilemap();

        }

    }

    private void CheckIfPlayerOnTreeTilemap()
    {
        Tilemap tilemap = ZoneManager.Instance.GetCurrentZoneHandler().TreeTilemap;
        if(lastTreeTilemap == null) lastTreeTilemap = tilemap;

        if(lastTreeTilemap != tilemap)
        {
            UnfadeAllFadedCells(lastTreeTilemap);
            lastTreeTilemap = tilemap;
        }



        Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.GetPlayerCenter());
        bool onTreeTile = tilemap.GetTile(playerCurrentCell) != null;


        if (onTreeTile && !isInForest)
        {
            StartCoroutine(EnterForest(tilemap));
        }

        else if (!onTreeTile && isInForest)
        {
            ExitForest(tilemap);
           
        }
    }

    private IEnumerator EnterForest(Tilemap tilemap)
    {
        isInForest= true;
        while(isInForest && playerController)
        {
            Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.transform.position) - new Vector3Int(0, 1, 0);

            FindAllCloseByCells(tilemap, playerCurrentCell);
            HandleAllCellsMarkedAsFaded(tilemap, playerCurrentCell);

            yield return null;
        }
        
       

    }

    private void ExitForest(Tilemap tilemap)
    {
        isInForest = false;
        UnfadeAllFadedCells(tilemap);
    
    }

    private void FindAllCloseByCells(Tilemap tilemap, Vector3Int playerCurrentCell)
    {
        
        for (int i = -forestCheckRadius; i <= forestCheckRadius; i++)
        {
            for (int j = -forestCheckRadius; j <= forestCheckRadius; j++)
            {
                Vector3Int cell = playerCurrentCell + new Vector3Int(j, i, 0);
                if (!tilemap.HasTile(cell) || fadedCells.Contains(cell)) continue;
                fadedCells.Add(cell);

            }
        }
      
    }

    private void HandleAllCellsMarkedAsFaded(Tilemap tilemap, Vector3Int playerCurrentCell)
    {
        
        float maxDistSqr = forestCheckRadius * forestCheckRadius;

        Vector3 playerWorldPos = tilemap.CellToWorld(playerCurrentCell);
        HashSet<Vector3Int> cellsToRemove = new();
        foreach (var cell in fadedCells)
        {
            Vector3 cellWorldPos = tilemap.CellToWorld(cell);
           
            //Close
            if ((playerWorldPos - cellWorldPos).sqrMagnitude <= maxDistSqr)
            {

                if (fadeCoroutinesDict.ContainsKey(cell))
                {
                    StopCoroutine(fadeCoroutinesDict[cell]);
                    fadeCoroutinesDict[cell] = null;
                    fadeCoroutinesDict[cell] = StartCoroutine(FadeTile(tilemap, cell, 0.20f, 0.3f));
                    
                }
                else
                {
                    fadeCoroutinesDict.Add(cell, StartCoroutine(FadeTile(tilemap, cell, 0.20f, 0.3f)));
                }
            }
            //Far
            else
            {
                if (fadeCoroutinesDict.ContainsKey(cell))
                {
                    StopCoroutine(fadeCoroutinesDict[cell]);
                    fadeCoroutinesDict[cell] = null;
                    fadeCoroutinesDict[cell] = StartCoroutine(FadeTile(tilemap, cell, 1f, 0.3f));

           
                }
                else
                {
                    fadeCoroutinesDict.Add(cell, StartCoroutine(FadeTile(tilemap, cell, 1f, 0.3f)));
                }


                cellsToRemove.Add(cell);
            }
        }

        foreach (var cell in cellsToRemove)
        {
            fadedCells.Remove(cell);
        }

    }

  
    private void UnfadeAllFadedCells(Tilemap tilemap)
    {
        HashSet<Vector3Int> cellsToRemove = new();
        foreach (var cell in fadedCells)
        {
           
            if (fadeCoroutinesDict.ContainsKey(cell))
            {
                StopCoroutine(fadeCoroutinesDict[cell]);
                fadeCoroutinesDict[cell] = null;

                fadeCoroutinesDict.Remove(cell);
            }
          
            Coroutine coroutine = StartCoroutine(FadeTile(tilemap, cell, 1f, 0.2f));
            fadeCoroutinesDict.Add(cell, coroutine);
            cellsToRemove.Add(cell);


        }
        foreach (var cell in cellsToRemove)
        {
            fadedCells.Remove(cell);
        }
        fadedCells.Clear();
        fadeCoroutinesDict.Clear();

    }

    private IEnumerator FadeTile(Tilemap tilemap, Vector3Int cellPos, float targetAlpha, float duration)
    {
        Color startColor = tilemap.GetColor(cellPos);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            tilemap.SetColor(cellPos, Color.Lerp(startColor, endColor, t));
            yield return null;
        }

        tilemap.SetColor(cellPos, endColor);
        fadeCoroutinesDict.Remove(cellPos);

    }


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void FixedUpdate()
    {
        CheckForFloorType();
    }

    private void CheckForFloorType()
    {
        RaycastHit2D rayCast = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);

        if (rayCast && !rayCast.collider.CompareTag(playerController.CurrentFloorType.ToString()))
        {
            playerController.PlayerRunState.StopRunningSFX();
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
