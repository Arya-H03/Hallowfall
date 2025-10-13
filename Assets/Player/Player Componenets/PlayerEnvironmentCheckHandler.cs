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

    PlayerController playerController;

    private readonly int treeCheckRadius = 4;


    private Tilemap treeTilemap;
    private HashSet<Vector3Int> fadedTiles = new();
    private HashSet<Vector3Int> tilesToFade = new();
    private HashSet<Vector3Int> tilesToUnfade = new();
    private Dictionary<Vector3Int, Coroutine> fadingTilesDict = new();



    public CDetector Detector { get => detector; }
    public LayerMask EnemyLayerMask { get => enemyLayer; }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        detector = GetComponent<CDetector>();
    }
    private void Start()
    {
        treeTilemap = ZoneManager.Instance.GlobalTreeTilemap;
    }
    private void FixedUpdate()
    {
        CheckForFloorType();
    }
    private void Update()
    {

        if (!playerController || playerController.IsDead) return;
                                                              
        Vector3 playerPos = playerController.GetPlayerPos() - new Vector3(0,2,0);

        CheckForNearbyTreeTiles(playerPos);
        HandleTiles(playerPos);
      


    }
    private void CheckForNearbyTreeTiles(Vector3 centerPos)
    {
        Vector3Int centerTilePos = new Vector3Int((int)centerPos.x, (int)centerPos.y, (int)centerPos.z);
        for (int i = -treeCheckRadius; i < treeCheckRadius; i++)
        {
            for (int j = -treeCheckRadius; j < treeCheckRadius; j++)
            {
                Vector3Int tilePos = centerTilePos + new Vector3Int(i, j, 0);

                if (!fadedTiles.Contains(tilePos)) tilesToFade.Add(tilePos);

            }
        }
    }
    private void HandleTiles(Vector3 centerPos)
    {
        float sqrRadius = treeCheckRadius * treeCheckRadius;

        // Fade tiles that should be faded
        foreach (Vector3Int tilePos in tilesToFade)
        {
            if (fadedTiles.Contains(tilePos)) continue;

            if ((centerPos - (Vector3)tilePos).sqrMagnitude <= sqrRadius)
            {
                StartFade(tilePos, 0.2f, 0.3f);
                fadedTiles.Add(tilePos);
            }
        }


        tilesToFade.Clear();


        // Unfade tiles that left the radius
        foreach (Vector3Int tilePos in fadedTiles)
        {
            if ((centerPos - (Vector3)tilePos).sqrMagnitude > sqrRadius)
            {
                tilesToUnfade.Add(tilePos);
            }
        }

        foreach (Vector3Int tilePos in tilesToUnfade)
        {
            StartFade(tilePos, 1f, 0.2f);
            fadedTiles.Remove(tilePos);
        }

        tilesToUnfade.Clear();
    }

    private void StartFade(Vector3Int tilePos,float targetAlpha, float duration)
    {
        if(fadingTilesDict.TryGetValue(tilePos,out Coroutine coroutine)) StopCoroutine(coroutine);
        
        fadingTilesDict[tilePos] = StartCoroutine(FadeTileCoroutine(treeTilemap, tilePos, targetAlpha, duration));
    }

    private IEnumerator FadeTileCoroutine(Tilemap tilemap, Vector3Int tilePos, float targetAlpha, float duration)
    {
        Color startColor = tilemap.GetColor(tilePos);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            tilemap.SetColor(tilePos, Color.Lerp(startColor, endColor, t));
            yield return null;
        }

        tilemap.SetColor(tilePos, endColor);
        fadingTilesDict.Remove(tilePos);
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
