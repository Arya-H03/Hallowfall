using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RitualClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid)
    {
        CellPaint grassTilePaint = new CellPaint {  tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = false };
        Cell blockCenterCell = cellGrid.GetCenterCellOfGrid();

        //Chalice
        cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.chalicePrefab, blockCenterCell.LocalCellCoord, Quaternion.identity, true, propsHolder.transform);

        //Candles
        List<Vector2Int> candleCoordList = new List<Vector2Int>();

        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(0, 2));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(1, 1));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(2, 0));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(1, -1));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(0, -2));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-1, -1));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-2, 0));
        candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-1, 1));

        foreach (Vector2Int candleCoord in candleCoordList)
        {
            cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.candlePrefab, candleCoord, Quaternion.identity, true, propsHolder.transform);
        }

        cellGrid.LoopOverGrid((i, j) =>
        {
            //Skulls             
            if (Random.value > 0.7f && !cellGrid.Cells[i, j].IsOccupied)
            {
                TileBase skullTile = MyUtils.GetRandomRef(zoneLayoutProfile.skullTiles);
                CellPaint skullTilePaint = new CellPaint { tilemap = zoneHandler.PropsNoCollisionTilemap, tileBase = skullTile };
                cellGrid.Cells[i, j].AddToCellPaint(skullTilePaint);
            }

        });

        cellGrid.AddToBlockPaints(zoneHandler.GroundOneTilemap, zoneLayoutProfile.grassRuletile, cellGrid, cellGrid.ParentCellGrid);


    }

  
}
