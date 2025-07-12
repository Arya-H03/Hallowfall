
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {

        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            TilePaint tilePaintLeavesOnGroundTile = new TilePaint { tilemap = ZoneManager.Instance.GroundTwoTilemap, tileBase = graveYardLayoutProfile.leavesRuleTile };
            TilePaint grassTilePaint = new TilePaint { tilemap = ZoneManager.Instance.GroundOneTilemap, tileBase = graveYardLayoutProfile.grassRuletile };

            for (int y = 0; y < cellGrid.CellPerCol; y++)
            {
                for (int x = 0; x < cellGrid.CellPerRow; x++)
                {

                    if((y >= 1 && y < cellGrid.CellPerCol -1) && (x >= 1 && x < cellGrid.CellPerRow - 1))
                    {
                        Vector3Int pos = new Vector3Int((int)celLGrid.Cells[x, y].CellPos.x, (int)celLGrid.Cells[x, y].CellPos.y, 0);
                        TileBase treeTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.treeTiles, false);

                        if (treeTilebase != null)
                        {
                            TilePaint tilePaintTree = new TilePaint { tilemap = ZoneManager.Instance.TreeTilemap, tileBase = treeTilebase };
                            cellGrid.Cells[x, y].AddToTilePaints(tilePaintTree);
                        }
                    }
                  
                    cellGrid.Cells[x, y].AddToTilePaints(tilePaintLeavesOnGroundTile);
                    cellGrid.Cells[x, y].AddToTilePaints(grassTilePaint);

                }
            }

        }
    }


}
