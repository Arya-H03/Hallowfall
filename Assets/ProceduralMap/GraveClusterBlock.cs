using UnityEngine;
using UnityEngine.Tilemaps;


public class GraveClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        TilePaint grassTielPaint = new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, tileBase = zoneLayoutProfile.grassRuletile };
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
         
            for (int y = 1; y < cellGrid.CellPerCol; y += 3)
            {
                for (int x = 1; x < cellGrid.CellPerRow - 1; x += 2)
                {
                    cellGrid.Cells[x, y].IsOccupied = true;

                    TileBase graveStoneTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.graveStoneTiles, false);
                    if (graveStoneTilebase != null)
                    {
                        TilePaint tilePaintGravestone = new TilePaint { tilemap = ZoneManager.Instance.PropsTilemap, tileBase = graveStoneTilebase };
                        cellGrid.Cells[x, y].AddToTilePaints(tilePaintGravestone);
                    }                  

                    TryAddGraveDirt(graveYardLayoutProfile, new Vector2Int(x,y-1),celLGrid);
                    TryAddSkulls(graveYardLayoutProfile, (Vector3Int)cellGrid.Cells[x, y-1].CellPos);

                }
            }

            for(int j = 0; j < cellGrid.CellPerCol; j++)
            {
                for (int i = 0; i < cellGrid.CellPerRow; i++) 
                {
               
                    cellGrid.Cells[i, j].AddToTilePaints(grassTielPaint);
                }
            }
        }
      
    }
    private void TryAddGraveDirt(GraveYardLayoutProfile graveYardLayoutProfile, Vector2Int cellCoord,CellGrid cellGrid)
    {
        if (Random.Range(1, 7) > 4)
        { 
            
            TileBase graveDirtTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.graveDirtTiles, false);
            if (graveDirtTilebase != null)
            {
                cellGrid.Cells[cellCoord.x, cellCoord.y].IsOccupied = true;
                TilePaint tilePaintGravestone = new TilePaint { tilemap = ZoneManager.Instance.GroundPropsTilemap, tileBase = graveDirtTilebase };
                cellGrid.Cells[cellCoord.x, cellCoord.y].AddToTilePaints(tilePaintGravestone);
            }
        }
    }
    private void TryAddSkulls(GraveYardLayoutProfile graveYardLayoutProfile,Vector3 cellPos)
    {
        if (Random.value > 0.75)
        {
            int skullCount = Random.Range(1, 3);
            for (int i = 0; i < skullCount; i++)
            {
                Vector2 offset = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(-0.4f, 0.4f));
                Vector3 pos = cellPos + (Vector3)offset;
                GameObject skullGO = Instantiate(graveYardLayoutProfile.skullPrefab, pos, Quaternion.identity);
                skullGO.GetComponent<SpriteRenderer>().sprite = graveYardLayoutProfile.GetRandomSprite(graveYardLayoutProfile.skullSprites);
                skullGO.transform.parent = propsHolder.transform;
                skullGO.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
            }
        }
    }
}
