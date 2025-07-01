
using UnityEngine;
using UnityEngine.Tilemaps;
public class TreeClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
       
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            TilePaint tilePaintLeaves =  new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, tileBase = graveYardLayoutProfile.leafRuleTile } ;

            for (int y = 0; y < cellGrid.CellPerCol; y++)
            {
                for (int x = 0; x < cellGrid.CellPerRow; x++)
                {
                                 
                    Vector3Int pos = new Vector3Int((int)celLGrid.Cells[x, y].CellPos.x, (int)celLGrid.Cells[x, y].CellPos.y, 0);
                    TileBase treeTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.treeTiles,true);
                    if (treeTilebase != null)
                    {
                        TilePaint tilePaintTree = new TilePaint { tilemap = ZoneManager.Instance.PropsTilemap, tileBase = treeTilebase };
                        cellGrid.Cells[x, y].AddToTilePaints(tilePaintTree);
                    }             
                    cellGrid.Cells[x, y].AddToTilePaints(tilePaintLeaves);
                 
                }
            }

        }
    }

  
}
