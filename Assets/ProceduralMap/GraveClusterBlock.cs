using UnityEngine;

public class GraveClusterBlock : PropsBlock
{
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        if(zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {
            for (int y = 1; y < cellGrid.CellPerCol; y += 3)
            {
                for (int x = 1; x < cellGrid.CellPerRow - 1; x += 2)
                {
                    GameObject propPrefab = graveYardLayoutProfile.GetRandomProps(graveYardLayoutProfile.gravestonePrefabs);

                    Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
                    Vector3 propsScale = new Vector3(bounds.size.x, bounds.size.y , 1);

                    GameObject prop = Instantiate(propPrefab, cellGrid.Cells[x, y].CellPos + new Vector2(propsScale.x / 2, 0), Quaternion.identity);
                    prop.transform.parent = propsHolder.transform;

                    for (int i = cellGrid.Cells[x, y].CellID.x; i < cellGrid.Cells[x, y].CellID.x + propsScale.x; i++)
                    {
                        for (int j = cellGrid.Cells[x, y].CellID.y; j < cellGrid.Cells[x, y].CellID.y + propsScale.y; j++)
                        {
                            if (i >= 0 && i < cellGrid.CellPerRow && j >= 0 && j < cellGrid.CellPerCol)
                            {
                                cellGrid.Cells[i, j].IsOccupied = true;
                            }
                        }
                    }

                }
            }
        }
      
    }
}
