using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RitualCluster : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        GraveYardLayoutProfile graveYardLayoutProfile = zoneLayoutProfile as GraveYardLayoutProfile;
        if(graveYardLayoutProfile)
        {
            TilePaint grassTilePaint = new TilePaint {/*tilemap = ZoneManager.Instance.GroundOneTilemap*/  tilemap = zoneHandler.GroundOneTilemap, tileBase = graveYardLayoutProfile.grassRuletile};
            Cell blockCenterCell = cellGrid.GetCenterCellOfGrid();

            //Chalice
            cellGrid.TryInstantiatePremanantGameobjectOnTile(graveYardLayoutProfile.chalicePrefab, blockCenterCell.LocalCellCoord,  Quaternion.identity, true, propsHolder.transform);

            //Candles
            List<Vector2Int> candleCoordList = new List<Vector2Int>();

            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(0,2));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(1,1));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(2,0));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(1,-1));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(0,-2));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-1,-1));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-2,0));
            candleCoordList.Add(blockCenterCell.LocalCellCoord + new Vector2Int(-1,1));

            foreach(Vector2Int candleCoord in candleCoordList)
            {
                cellGrid.TryInstantiatePremanantGameobjectOnTile(graveYardLayoutProfile.candlePrefab, candleCoord, Quaternion.identity, true, propsHolder.transform);
            }

            cellGrid.LoopOverGrid((i, j) =>
            {
                //Skulls             
                if(Random.value > 0.7f)
                {
                    GameObject skullGO = cellGrid.TryInstantiatePremanantGameobjectOnTile(graveYardLayoutProfile.skullPrefab, cellGrid.Cells[i, j].LocalCellCoord, Quaternion.identity, true, propsHolder.transform);
                    if(skullGO)
                    {
                        Vector2 offset = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(-0.4f, 0.4f));
                        Vector3 pos = cellGrid.Cells[i, j].GlobalCellPos + offset;

                        skullGO.GetComponent<SpriteRenderer>().sprite = graveYardLayoutProfile.GetRandomSprite(graveYardLayoutProfile.skullSprites);
                        skullGO.transform.position = pos;
                        skullGO.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
                    }                   
                }

                cellGrid.Cells[i, j].AddToTilePaints(grassTilePaint);
            });
        }
      
    }

  
}
