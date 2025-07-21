
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeClusterBlock : PropsBlock
{
    //private bool isInForest = false;
    //private readonly int forestCheckRadius = 4;
    //private PlayerController playerController;

    ///////////////////
    private void Start()
    {

        //p/*layerController = GameManager.Instance.PlayerController;*/
    }
    protected override void PopulateBlock(CellGrid cellGrid)
    {
        CellPaint tilePaintLeavesOnGroundTile = new CellPaint {tilemap = zoneHandler.GroundTwoTilemap, tileBase = zoneLayoutProfile.leavesRuleTile };
        CellPaint grassTilePaint = new CellPaint {tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = false };

        for (int y = 0; y < cellGrid.CellPerCol; y++)
        {
            for (int x = 0; x < cellGrid.CellPerRow; x++)
            {
                Vector3Int pos = new Vector3Int((int)celLGrid.Cells[x, y].GlobalCellPos.x, (int)celLGrid.Cells[x, y].GlobalCellPos.y, 0);
                TileBase treeTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.treeTiles, false);

                if (treeTilebase != null)
                {
                    CellPaint tilePaintTree = new CellPaint { tilemap = zoneHandler.TreeTilemap, tileBase = treeTilebase };
                    cellGrid.Cells[x, y].AddToCellPaint(tilePaintTree);
                }
                //if (/*Random.value > 1 - zoneLayoutProfile.treeDensity && */y >= 1)
                //{
                   
                //}

                //if ((y >= 1 && y < cellGrid.CellPerCol -1) && (x >= 1 && x < cellGrid.CellPerRow - 1))
                //{
                //    Vector3Int pos = new Vector3Int((int)celLGrid.Cells[x, y].GlobalCellPos.x, (int)celLGrid.Cells[x, y].GlobalCellPos.y, 0);
                //    TileBase treeTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.treeTiles, false);

                //    if (treeTilebase != null)
                //    {
                //        CellPaint tilePaintTree = new CellPaint { tilemap = ZoneManager.Instance.TreeTilemap, tileBase = treeTilebase };
                //        cellGrid.Cells[x, y].AddToCellPaint(tilePaintTree);
                //    }
                //}


                //cellGrid.Cells[x, y].AddToCellPaint(tilePaintLeavesOnGroundTile);
                //cellGrid.Cells[x, y].AddToCellPaint(grassTilePaint);

            }
        }

        cellGrid.AddToBlockPaints(zoneHandler.GroundTwoTilemap, zoneLayoutProfile.leavesRuleTile, cellGrid, cellGrid.ParentCellGrid);
        cellGrid.AddToBlockPaints(zoneHandler.GroundOneTilemap, zoneLayoutProfile.grassRuletile, cellGrid, cellGrid.ParentCellGrid);
    }

    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.CompareTag("Player"))
    //    {
    //        EnterForest(zoneHandler.TreeTilemap);
    //    }
    //}

    //protected override void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        ExitForest();
    //    }
    //}

    //private IEnumerator WhileInForestCouroutine(Tilemap tilemap)
    //{
    //    HashSet<Vector3Int> fadedCells = new HashSet<Vector3Int>();
    //    List<Vector3Int> cellsToUnfade = new List<Vector3Int>();
    //    float maxDistSqr = forestCheckRadius * forestCheckRadius;

    //    while (isInForest && playerController)
    //    {
    //        Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.transform.position) - new Vector3Int(0, 1, 0);
    //        Vector3 playerWorldPos = tilemap.CellToWorld(playerCurrentCell);

    //        // Fade new tiles
    //        for (int i = -forestCheckRadius; i <= forestCheckRadius; i++)
    //        {
    //            for (int j = -forestCheckRadius; j <= forestCheckRadius; j++)
    //            {
    //                Vector3Int cellPos = playerCurrentCell + new Vector3Int(j, i, 0);
    //                TileBase tileBase = tilemap.GetTile(cellPos);

    //                if (tileBase != null && fadedCells.Add(cellPos)) // Add only if not already present
    //                {
    //                    tilemap.SetTileFlags(cellPos, TileFlags.None);
    //                    StartCoroutine(FadeTile(tilemap, cellPos, 0.20f, 0.1f));
    //                }
    //            }
    //        }

    //        // Unfade far tiles
    //        cellsToUnfade.Clear();
    //        foreach (var cell in fadedCells)
    //        {
    //            Vector3 cellWorldPos = tilemap.CellToWorld(cell);
    //            if ((playerWorldPos - cellWorldPos).sqrMagnitude > maxDistSqr)
    //            {
    //                cellsToUnfade.Add(cell);
    //            }
    //        }

    //        foreach (var cell in cellsToUnfade)
    //        {
    //            StartCoroutine(FadeTile(tilemap, cell, 1f, 0.1f));
    //            fadedCells.Remove(cell);
    //        }

    //        yield return new WaitForSeconds(0.2f);
    //    }

    //    // On forest exit
    //    foreach (var cell in fadedCells)
    //    {
    //        StartCoroutine(FadeTile(tilemap, cell, 1f, 0.2f));
    //    }

    //    fadedCells.Clear();
    //}

    //private IEnumerator FadeTile(Tilemap tilemap, Vector3Int cellPos, float targetAlpha, float duration)
    //{
    //    Color startColor = tilemap.GetColor(cellPos);
    //    Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

    //    float timer = 0;
    //    while (timer < duration)
    //    {
    //        timer += Time.deltaTime;
    //        float t = timer / duration;
    //        Color lerpedColor = Color.Lerp(startColor, endColor, t);
    //        tilemap.SetColor(cellPos, lerpedColor);
    //        yield return null;
    //    }
    //    tilemap.SetColor(cellPos, endColor);
    //}

    //private void EnterForest(Tilemap tilemap)
    //{
    //    isInForest = true;
    //    //p/*layerController.PlayerMovementManager.SpeedModifer = 0.75f;*/

    //    //PlayerCamera.Instance.Vignette.intensity.Override(0.8f);
    //    //PlayerCamera.Instance.ColorAdjustments.contrast.Override(20);

    //    StartCoroutine(WhileInForestCouroutine(tilemap));
    //}

    //private void ExitForest()
    //{
    //    isInForest = false;
    //    //playerController.PlayerMovementManager.SpeedModifer = 1f;
    //    //PlayerCamera.Instance.Vignette.intensity.Override(0f);
    //    //PlayerCamera.Instance.ColorAdjustments.contrast.Override(0);
    //}

}
