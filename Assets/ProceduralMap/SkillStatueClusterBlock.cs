using UnityEngine;

public class SkillStatueClusterBlock : PropsBlock
{
    protected override void PopulateBlock(SubCellGrid subCellGrid)
    {
        CellPaint tilePaintStone = new CellPaint { tilemap = zoneHandler.GroundTwoTilemap, tileBase = zoneLayoutProfile.stoneRoadRuleTile };
        CellPaint tilePaintGrass = new CellPaint { tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = false };

        Cell centerBlockCell = subCellGrid.GetCenterCellOfGrid();

        GameObject statue = Instantiate(zoneLayoutProfile.skillStatuePrefab, (Vector3Int)centerBlockCell.GlobalCellPos, Quaternion.identity);
        statue.transform.parent = propsHolder.transform;

        for (int j = 0; j < subCellGrid.CellPerCol; j++)
        {
            for (int i = 0; i < subCellGrid.CellPerRow; i++)
            {
                if(j >= centerBlockCell.LocalCellCoord.y - 2 && j < centerBlockCell.LocalCellCoord.y +2 &&
                i >= centerBlockCell.LocalCellCoord.x - 2 && i < centerBlockCell.LocalCellCoord.x +2) 
                {
                    subCellGrid.Cells[i, j].MarkAsOccupied();
                    subCellGrid.Cells[i, j].MarkAsUnwalkable();
                    subCellGrid.Cells[i, j].AddToCellPaint(tilePaintStone);
                }
                subCellGrid.Cells[i, j].AddToCellPaint(tilePaintGrass);

            }
        }

    }

}
