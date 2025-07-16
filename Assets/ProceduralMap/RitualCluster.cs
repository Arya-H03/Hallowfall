using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RitualCluster : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid)
    {
        TilePaint grassTilePaint = new TilePaint {  tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = true };
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
            if (Random.value > 0.7f)
            {
                GameObject skullGO = cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.skullPrefab, cellGrid.Cells[i, j].LocalCellCoord, Quaternion.identity, true, propsHolder.transform);
                if (skullGO)
                {
                    Vector3 offset = new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.4f, 0.4f), 0);
                    Vector3 pos = cellGrid.Cells[i, j].GlobalCellPos + offset;

                    skullGO.GetComponent<SpriteRenderer>().sprite = zoneLayoutProfile.GetRandomSprite(zoneLayoutProfile.skullSprites);
                    skullGO.transform.position = pos;
                    skullGO.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
                }
            }

            cellGrid.Cells[i, j].AddToTilePaints(grassTilePaint);
        });
      
      
    }

  
}
