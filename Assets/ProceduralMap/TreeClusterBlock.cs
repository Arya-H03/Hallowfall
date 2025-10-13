
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeClusterBlock : PropsBlock
{
 
    protected override void PopulateBlock(SubCellGrid subCellGrid)
    {
        CellPaint tilePaintLeavesOnGroundTile = new CellPaint {tilemap = zoneHandler.GroundTwoTilemap, tileBase = zoneLayoutProfile.leavesRuleTile };
        CellPaint grassTilePaint = new CellPaint {tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = false };

        Tilemap treeTilemap = ZoneManager.Instance.GlobalTreeTilemap;

        for (int y = 0; y < subCellGrid.CellPerCol; y++)
        {
            for (int x = 0; x < subCellGrid.CellPerRow; x++)
            {
                Vector3Int pos = new Vector3Int((int)base.subCellGrid.Cells[x, y].GlobalCellPos.x, (int)base.subCellGrid.Cells[x, y].GlobalCellPos.y, 0);
                TileBase treeTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.treeTiles, false);

                if (treeTilebase != null)
                {
                    CellPaint tilePaintTree = new CellPaint { tilemap = treeTilemap, tileBase = treeTilebase, isOnGlobalTile = true};
                    subCellGrid.Cells[x, y].AddToCellPaint(tilePaintTree);
                }
                //if (/*Random.value > 1 - zoneLayoutProfile.treeDensity && */y >= 1)
                //{
                   
                //}

                //if ((y >= 1 && y < subCellGrid.CellPerCol -1) && (x >= 1 && x < subCellGrid.CellPerRow - 1))
                //{
                //    Vector3Int pos = new Vector3Int((int)subCellGrid.Cells[x, y].GlobalCellPos.x, (int)subCellGrid.Cells[x, y].GlobalCellPos.y, 0);
                //    TileBase treeTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.treeTiles, false);

                //    if (treeTilebase != null)
                //    {
                //        CellPaint tilePaintTree = new CellPaint { tilemap = ZoneManager.Instance.TreeTilemap, tileBase = treeTilebase };
                //        subCellGrid.Cells[x, y].AddToCellPaint(tilePaintTree);
                //    }
                //}

            }
        }

        subCellGrid.AddToBlockPaints(zoneHandler.GroundTwoTilemap, zoneLayoutProfile.leavesRuleTile, subCellGrid, subCellGrid.ParentCellGrid);
        subCellGrid.AddToBlockPaints(zoneHandler.GroundOneTilemap, zoneLayoutProfile.grassRuletile, subCellGrid, subCellGrid.ParentCellGrid);
    }
}
