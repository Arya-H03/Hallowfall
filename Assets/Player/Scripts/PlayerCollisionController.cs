using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCollisionController : MonoBehaviour
{
    private PlayerController playerController;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;



    ///Forest Detector
    private Tilemap treeTilemap;
    private bool isInForest = false;
    private readonly int forestCheckRadius = 4;

    ///////////////////


    public BoxCollider2D BoxCollider2D { get => boxCollider2D; private set => boxCollider2D = value; }
    public Rigidbody2D Rb { get => rb; private set => rb = value; }


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        treeTilemap = ZoneManager.Instance.TreeTilemap;
    }

    private void Update()
    {
        CheckIfPlayerOnTreeTilemap(treeTilemap);

    }

    public void KnockPlayer(Vector2 launchVel)
    {
        playerController.rb.linearVelocity += launchVel;
    }
    private void CheckIfPlayerOnTreeTilemap(Tilemap tilemap)
    {
        Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.GetPlayerCenter());
        bool onTreeTile = tilemap.GetTile(playerCurrentCell) != null;

        //Enters Forest
        if (onTreeTile && !isInForest)
        {
            OnEnterForest(tilemap);
        }
        //Exists Forest
        else if (!onTreeTile && isInForest)
        {
            OnExitForest();  
        }
    }
    private IEnumerator WhileInForestCouroutine(Tilemap tilemap)
    {
        HashSet<Vector3Int> fadedCells = new HashSet<Vector3Int>();
        List<Vector3Int> cellsToUnfade = new List<Vector3Int>();
        float maxDistSqr = forestCheckRadius * forestCheckRadius;

        while (isInForest)
        {
            Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.transform.position) - new Vector3Int(0, 1, 0);
            Vector3 playerWorldPos = tilemap.CellToWorld(playerCurrentCell);

            // Fade new tiles
            for (int i = -forestCheckRadius; i <= forestCheckRadius; i++)
            {
                for (int j = -forestCheckRadius; j <= forestCheckRadius; j++)
                {
                    Vector3Int cellPos = playerCurrentCell + new Vector3Int(j, i, 0);
                    TileBase tileBase = tilemap.GetTile(cellPos);

                    if (tileBase != null && fadedCells.Add(cellPos)) // Add only if not already present
                    {
                        tilemap.SetTileFlags(cellPos, TileFlags.None);
                        StartCoroutine(FadeTile(tilemap, cellPos, 0.20f, 0.2f));
                    }
                }
            }

            // Unfade far tiles
            cellsToUnfade.Clear();
            foreach (var cell in fadedCells)
            {
                Vector3 cellWorldPos = tilemap.CellToWorld(cell);
                if ((playerWorldPos - cellWorldPos).sqrMagnitude > maxDistSqr)
                {
                    cellsToUnfade.Add(cell);
                }
            }

            foreach (var cell in cellsToUnfade)
            {
                StartCoroutine(FadeTile(tilemap, cell, 1f, 0.2f));
                fadedCells.Remove(cell);
            }

            yield return new WaitForSeconds(0.4f);
        }

        // On forest exit
        foreach (var cell in fadedCells)
        {
            StartCoroutine(FadeTile(tilemap, cell, 1f, 0.2f));
        }

        fadedCells.Clear();
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
            Color lerpedColor = Color.Lerp(startColor, endColor, t);
            tilemap.SetColor(cellPos, lerpedColor);
            yield return null;
        }
        tilemap.SetColor(cellPos, endColor);
    }

    private void OnEnterForest(Tilemap tilemap)
    {
        isInForest = true;
        playerController.PlayerMovementManager.SpeedModifer = 0.75f;

        PlayerCamera.Instance.Vignette.intensity.Override(0.8f);
        PlayerCamera.Instance.ColorAdjustments.contrast.Override(20);

        StartCoroutine(WhileInForestCouroutine(tilemap));
    }

    private void OnExitForest()
    {
        isInForest = false;
        playerController.PlayerMovementManager.SpeedModifer = 1f;
        PlayerCamera.Instance.Vignette.intensity.Override(0f);
        PlayerCamera.Instance.ColorAdjustments.contrast.Override(0);
    }

}



