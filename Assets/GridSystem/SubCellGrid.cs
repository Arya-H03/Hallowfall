using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SubCellGrid : CellGrid
{
    private CellGrid parentCellGrid;

    public CellGrid ParentCellGrid => parentCellGrid;

    public SubCellGrid(CellGrid parentCellGrid, int subGridWidth, int subGridHeight, Vector3Int originCellWorldPos)
        : base(parentCellGrid.CellSize, subGridWidth, subGridHeight)
    {
        this.parentCellGrid = parentCellGrid;

        this.blockPaintList = parentCellGrid.BlockPaintList;

        cells = new Cell[CellPerRow, CellPerCol];

        InitializeSubGridCells(originCellWorldPos);
    }

    private void InitializeSubGridCells(Vector3Int originCellWorldPos)
    {
        Vector3Int offset = originCellWorldPos - parentCellGrid.Cells[0, 0].GlobalCellPos;
        Vector2Int originCellCoord = new Vector2Int(offset.x / CellSize, offset.y / CellSize);

        Cell originCell = parentCellGrid.Cells[originCellCoord.x, originCellCoord.y];

        for (int i = 0; i < CellPerRow; i++)
        {
            for (int j = 0; j < CellPerCol; j++)
            {
                Cell parentCell = parentCellGrid.Cells[i + originCell.GlobalCellCoord.x, j + originCell.GlobalCellCoord.y];
                parentCell.LocalCellCoord = parentCell.GlobalCellCoord - originCell.GlobalCellCoord;
                cells[i, j] = parentCell;
            }
        }
    }
}

