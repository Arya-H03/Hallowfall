using UnityEngine;
using UnityEngine.Tilemaps;

public class CryptClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            GameObject cryptPrefab = MyUtils.GetRandomRef<GameObject>(graveYardLayoutProfile.cryptPrefabs);

            Bounds bounds = MyUtils.GetCombinedBounds(cryptPrefab);

            int xBound = Mathf.CeilToInt(bounds.size.x);
            int yBound = Mathf.CeilToInt(bounds.size.y);



            int x = Mathf.CeilToInt(((cellGrid.GridWidth - xBound) / (cellGrid.CellSize * 2)) + (xBound / 2));
            int y = Mathf.CeilToInt(((cellGrid.GridHeight - yBound) / (cellGrid.CellSize * 2)) + (yBound / 2));

            GameObject crypt = Instantiate(cryptPrefab, (Vector3Int)cellGrid.Cells[x, y].CellPos, Quaternion.identity);
            crypt.transform.parent = propsHolder.transform;

            int minX = x - Mathf.CeilToInt(xBound / 2);
            int maxX = x + Mathf.FloorToInt(xBound / 2);
            int minY = y;
            int maxY = y + Mathf.FloorToInt(yBound / 2);

            cellGrid.TryInstantiateGameobjectOnTile(graveYardLayoutProfile.flameHolderPrefab, new Vector2Int(minX - 1, minY - 1), Quaternion.identity, true, crypt.transform);
            cellGrid.TryInstantiateGameobjectOnTile(graveYardLayoutProfile.flameHolderPrefab, new Vector2Int(maxX, minY - 1), Quaternion.identity, true, crypt.transform);
            cellGrid.TryInstantiateGameobjectOnTile(graveYardLayoutProfile.flameHolderPrefab, new Vector2Int(minX - 1, maxY ), Quaternion.identity, true, crypt.transform);
            cellGrid.TryInstantiateGameobjectOnTile(graveYardLayoutProfile.flameHolderPrefab, new Vector2Int(maxX , maxY ), Quaternion.identity, true, crypt.transform);

            for (int j = 0; j < cellGrid.CellPerCol; j++)
            {
                for (int i = 0; i < cellGrid.CellPerRow; i++)
                {
                    bool isInsideCryptArea = i >= minX && i < maxX && j >= minY && j < maxY;
                    bool isAroundCryptArea = i >= minX - 1 && i < maxX + 1 && j >= minY - 1 && j < maxY + 1;

                    if (isInsideCryptArea)
                    {
                        cellGrid.Cells[i, j].IsOccupied = true;
                    }
                    //if (!isInsideCryptArea && Random.value < graveYardLayoutProfile.clutterDensity)
                    //{
                    //    GameObject skullSpikesPrefab = graveYardLayoutProfile.GetRandomProps(graveYardLayoutProfile.skullSpikesPrefabs);
                    //    GameObject skullOnSpike = Instantiate(skullSpikesPrefab, (Vector3Int)cellGrid.Cells[i, j].CellPos, Quaternion.identity);
                    //    skullOnSpike.transform.parent = propsHolder.transform;
                    //    cellGrid.Cells[i, j].IsOccupied = true;

                    //}
                    if (isAroundCryptArea)
                    {

                        TilePaint tilePaintStone = new TilePaint { tilemap = ZoneManager.Instance.GroundPropsTilemap, tileBase = graveYardLayoutProfile.roadRuletile };
                        cellGrid.Cells[i, j].AddToTilePaints(tilePaintStone);

                    }
                    else
                    {
                        TilePaint tilePaintGrass = new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, tileBase = graveYardLayoutProfile.grassRuletile };
                        cellGrid.Cells[i, j].AddToTilePaints(tilePaintGrass);
                    }


                }
            }




        }
    }
}
