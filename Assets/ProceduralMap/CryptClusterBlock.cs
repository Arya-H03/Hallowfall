using UnityEngine;

public class CryptClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid)
    {
        TilePaint tilePaintStone = new TilePaint {tilemap = zoneHandler.GroundTwoTilemap, tileBase = zoneLayoutProfile.stoneRoadRuleTile };
            TilePaint tilePaintGrass = new TilePaint {tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile,isOnGlobalTile = true };


            GameObject cryptPrefab = MyUtils.GetRandomRef<GameObject>(zoneLayoutProfile.cryptPrefabs);
            Bounds cryptBounds = MyUtils.GetCombinedBounds(cryptPrefab);
        
            Cell centerBlockCell = cellGrid.GetCenterCellOfGrid();

            GameObject crypt = Instantiate(cryptPrefab, (Vector3Int)centerBlockCell.GlobalCellPos, Quaternion.identity);
            crypt.transform.parent = propsHolder.transform;


            //4 corners of the crypt
            int minX = centerBlockCell.LocalCellCoord.x - Mathf.FloorToInt(cryptBounds.size.x / 2);
            int maxX = centerBlockCell.LocalCellCoord.x + Mathf.FloorToInt(cryptBounds.size.x / 2);
            int minY = centerBlockCell.LocalCellCoord.y;
            int maxY = centerBlockCell.LocalCellCoord.y + Mathf.FloorToInt(cryptBounds.size.y / 2);



            //Flame Holders
            cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.flameHolderPrefab, new Vector2Int(minX - 1, minY - 1), Quaternion.identity, true, crypt.transform);          
            cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.flameHolderPrefab, new Vector2Int(maxX, minY - 1), Quaternion.identity, true, crypt.transform);
            cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.flameHolderPrefab, new Vector2Int(minX - 1, maxY ), Quaternion.identity, true, crypt.transform);
            cellGrid.TryInstantiatePermanentGameobjectOnTile(zoneLayoutProfile.flameHolderPrefab, new Vector2Int(maxX , maxY ), Quaternion.identity, true, crypt.transform);

           

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
                    //    GameObject skullOnSpike = Instantiate(skullSpikesPrefab, (Vector3Int)cellGrid.Cells[i, j].GlobalCellPos, Quaternion.identity);
                    //    skullOnSpike.transform.parent = propsHolder.transform;
                    //    cellGrid.Cells[i, j].IsOccupied = true;

                    //}
                    if (isAroundCryptArea)
                    {                    
                        cellGrid.Cells[i, j].AddToTilePaints(tilePaintStone);

                    }
                    else
                    {                      
                        cellGrid.Cells[i, j].AddToTilePaints(tilePaintGrass);
                    }


                }
            }

    }
}
