using UnityEngine;

public class TreeClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            for (int y = 0; y < cellGrid.CellPerCol; y++)
            {
                for (int x = 0; x < cellGrid.CellPerRow; x++)
                {
                    GameObject propPrefab = graveYardLayoutProfile.GetRandomProps(graveYardLayoutProfile.treePrefabs);

                    Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
                    Vector3 propsScale = new Vector3(bounds.size.x, bounds.size.y , 1);

                    GameObject prop = Instantiate(propPrefab, cellGrid.Cells[x, y].CellPos, Quaternion.identity);
                    prop.transform.parent = propsHolder.transform;

                    for (int i = cellGrid.Cells[x, y].CellID.x; i < cellGrid.Cells[x, y].CellID.x + propsScale.x; i++)
                    {
                        for (int j = cellGrid.Cells[x, y].CellID.y; j < cellGrid.Cells[x, y].CellID.y + propsScale.y; j++)
                        {
                            if (i >= 0 && i < cellGrid.CellPerRow && j >= 0 && j < cellGrid.CellPerCol)
                            {
                                cellGrid.Cells[i, j].IsOccupied = true;

                                GroundTilemap.SetTile(new Vector3Int((int)cellGrid.Cells[x, y].CellPos.x - (int)GroundTilemap.transform.position.x, (int)cellGrid.Cells[x, y].CellPos.y -(int)GroundTilemap.transform.position.y, 0), graveYardLayoutProfile.leaveRuletile);
                            }
                        }
                    }

                }
            }

        }
    }

  
}
