using UnityEngine;
using UnityEngine.Tilemaps;

public class CryptClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            GameObject propPrefab = graveYardLayoutProfile.cryptPrefab;
            Tilemap stoneTilemap = zoneHandler.CreateTilemap(zoneLayoutProfile.roadTilemapGO, this.transform);

            Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
            int xBound = Mathf.CeilToInt(bounds.size.x);
            int yBound = Mathf.CeilToInt(bounds.size.y);

          

            int x = Mathf.CeilToInt(((cellGrid.GridWidth - xBound) / (cellGrid.CellSize *2)) + (xBound / 2));
            int y = Mathf.CeilToInt(((cellGrid.GridHeight - yBound) / (cellGrid.CellSize *2)) + (yBound / 2));
      
            GameObject prop = Instantiate(propPrefab, cellGrid.Cells[x,y].CellPos, Quaternion.identity);
            prop.transform.parent = propsHolder.transform;

            int minX = x - Mathf.CeilToInt(xBound /2);
            int maxX = x + Mathf.FloorToInt(xBound / 2);
            int minY = y;
            int maxY = y + Mathf.FloorToInt(yBound / 2);

            for (int j = 0; j < cellGrid.CellPerCol; j++)
            {
                for (int i = 0; i < cellGrid.CellPerRow; i++)
                {
                    bool isInsideCryptArea = i >= minX && i < maxX && j >= minY && j < maxY;
                    bool isAroundCryptArea = i >= minX -1 && i < maxX +1 && j >= minY-1 && j < maxY+1;

                    if (!isInsideCryptArea && Random.value < graveYardLayoutProfile.clutterDensity)
                    {
                        GameObject skullSpikesPrefab = graveYardLayoutProfile.GetRandomProps(graveYardLayoutProfile.skullSpikesPrefabs);
                        GameObject skullOnSpike = Instantiate(skullSpikesPrefab, cellGrid.Cells[i, j].CellPos, Quaternion.identity);
                        skullOnSpike.transform.parent = propsHolder.transform;
                        cellGrid.Cells[i, j].IsOccupied = true;

                    }
                    if(isAroundCryptArea)
                    {
                        GroundTilemap.SetTile(new Vector3Int((int)cellGrid.Cells[i, j].CellPos.x , (int)cellGrid.Cells[i, j].CellPos.y , 0), null);
                        stoneTilemap.SetTile(new Vector3Int((int)cellGrid.Cells[i, j].CellPos.x - (int)stoneTilemap.transform.position.x, (int)cellGrid.Cells[i, j].CellPos.y - (int)stoneTilemap.transform.position.y, 0), graveYardLayoutProfile.roadRuletile);
                    }
                   
                }
            } 


           

        }
    }
}
