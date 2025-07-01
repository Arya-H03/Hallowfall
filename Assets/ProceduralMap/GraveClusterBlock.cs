using UnityEngine;
using static Cell;

public class GraveClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        if(zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            TilePaint[] tilePaints = { new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, ruleTile = zoneLayoutProfile.grassRuletile }, new TilePaint { tilemap = ZoneManager.Instance.PropsTilemap, ruleTile = graveYardLayoutProfile.graveStoneRuletile } };
            for (int y = 1; y < cellGrid.CellPerCol; y += 3)
            {
                for (int x = 1; x < cellGrid.CellPerRow - 1; x += 2)
                {
                    //ZoneManager.Instance.PropsTilemap.SetTile((Vector3Int)cellGrid.Cells[x, y].CellPos, graveYardLayoutProfile.graveStoneRuletile);
                    cellGrid.Cells[x, y].IsOccupied = true;
                    cellGrid.Cells[x, y].AddToTilePaints(tilePaints);
                    TryAddGraveDirt(graveYardLayoutProfile, new Vector2Int(x,y-1),celLGrid);
                    //GameObject propPrefab = GenerateRandomGraveStone(graveYardLayoutProfile);
                    //Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
                    //Vector3 propsScale = new Vector3(bounds.size.x, bounds.size.y , 1);

                    //GameObject prop = Instantiate(propPrefab, cellGrid.Cells[x, y].CellPos + new Vector2(propsScale.x / 2, 0), Quaternion.identity);
                    //prop.transform.parent = propsHolder.transform;
                    //TryAddGraveDirt(graveYardLayoutProfile, prop);
                    //TryAddSkulls(graveYardLayoutProfile, prop);
                    //for (int i = cellGrid.Cells[x, y].CellID.x; i < cellGrid.Cells[x, y].CellID.x + propsScale.x; i++)
                    //{
                    //    for (int j = cellGrid.Cells[x, y].CellID.y; j < cellGrid.Cells[x, y].CellID.y + propsScale.y; j++)
                    //    {
                    //        if (i >= 0 && i < cellGrid.CellPerRow && j >= 0 && j < cellGrid.CellPerCol)
                    //        {
                    //            cellGrid.Cells[i, j].IsOccupied = true;
                    //        }
                    //    }
                    //}

                }
            }
        }
      
    }

    private GameObject GenerateRandomGraveStone(GraveYardLayoutProfile graveYardLayoutProfile)
    {
        GameObject go = graveYardLayoutProfile.gravestoneBasePrefab;
        if (graveYardLayoutProfile.graveStoneSprites.Length > 0 && graveYardLayoutProfile.graveDirtSprites.Length > 0)
        {
            go.GetComponent<SpriteRenderer>().sprite = graveYardLayoutProfile.graveStoneSprites[Random.Range(0, graveYardLayoutProfile.graveStoneSprites.Length)];

           

          
        }

        return go;
    }
    private void TryAddGraveDirt(GraveYardLayoutProfile graveYardLayoutProfile, Vector2Int cellCoord,CellGrid cellGrid)
    {
        if (Random.Range(1, 7) > 4)
        {
            TilePaint[] tilePaints = { new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, ruleTile = zoneLayoutProfile.grassRuletile }, new TilePaint { tilemap = ZoneManager.Instance.PropsTilemap, ruleTile = graveYardLayoutProfile.graveDirstRuletile } };
            //ZoneManager.Instance.PropsTilemap.SetTile((Vector3Int)cellGrid.Cells[cellCoord.x, cellCoord.y].CellPos, graveYardLayoutProfile.graveDirstRuletile);
            cellGrid.Cells[cellCoord.x, cellCoord.y].AddToTilePaints(tilePaints);
        }
    }
    private void TryAddSkulls(GraveYardLayoutProfile graveYardLayoutProfile,GameObject graveGO)
    {
        if (Random.value > 0.75)
        {
            int skullCount = Random.Range(1, 3);
            for (int i = 0; i < skullCount; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 0.5f;
                Vector3 pos = graveGO.transform.GetChild(1).transform.position + new Vector3(offset.x, offset.y, 0);
                GameObject skullGO = Instantiate(graveYardLayoutProfile.skullPrefab, pos, Quaternion.identity);
                skullGO.GetComponent<SpriteRenderer>().sprite = graveYardLayoutProfile.GetRandomSprite(graveYardLayoutProfile.skullSprites);
                skullGO.transform.parent = graveGO.transform.GetChild(1).transform;
                skullGO.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
            }
        }
    }
}
