using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cell
{
    private bool isOccupied = false;
    private Vector2Int cellID = Vector2Int.zero;
    private Vector2 cellPos = Vector2.zero;

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public Vector2Int CellID { get => cellID; set => cellID = value; }
    public Vector2 CellPos { get => cellPos; set => cellPos = value; }

    public Cell(bool isOccupied, Vector2Int cellID, Vector2 zoneCenterPos, int cellSize, int gridW, int gridY)
    {
        this.isOccupied = isOccupied;
        this.cellID = cellID;
        this.cellPos = zoneCenterPos + new Vector2(-gridW / 2, -gridY / 2) + new Vector2(cellID.x * cellSize, cellID.y * cellSize);
    }
    public Cell() { }
    public bool CheckIfAllNeighboorsAreOccupied(CellGrid cellGrid)
    {

        Vector2Int[] allDirections = ProceduralUtils.GetAllDirections();

    
        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = CellID + dir;
            if (neighboorID.x >= 0 && neighboorID.x < cellGrid.CellPerRow && neighboorID.y >= 0 && neighboorID.y < cellGrid.CellPerCol)
            {
                Cell neighboor = cellGrid.Cells[neighboorID.x, neighboorID.y];
                if (neighboor.IsOccupied) return true;
            }
        }

        return false;
    }
    public bool CheckIfCardinalNeighboorsAreOccupied(CellGrid cellGrid)
    {

        Vector2Int[] allDirections = ProceduralUtils.GetCardinalDirections();

    

        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = CellID + dir;
            if (neighboorID.x >= 0 && neighboorID.x < cellGrid.CellPerRow && neighboorID.y >= 0 && neighboorID.y < cellGrid.CellPerCol)
            {
                Cell neighboor = cellGrid.Cells[neighboorID.x, neighboorID.y];
                if (neighboor.IsOccupied) return true;
            }
        }

        return false;
    }

}
public class CellGrid
{
    private int cellSize = 1;
    private int gridWidth = 1;
    private int gridHeight = 1;

    private int cellPerRow = 1;
    private int cellPerCol = 1;

    private Cell [,] cells = null;
    

    public CellGrid(int cellSize, int gridWidth, int gridHeight,Vector2 gridCenterWoldPos)
    {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        this.cellPerRow = Mathf.FloorToInt(this.GridWidth / cellSize);
        this.cellPerCol = Mathf.FloorToInt(this.GridHeight / cellSize);

        cells = new Cell[CellPerRow, CellPerCol];
        InitializeCells(gridCenterWoldPos);


    }

    public Cell[,] Cells { get => cells; set => cells = value; }
    public int CellPerCol { get => cellPerCol; set => cellPerCol = value; }
    public int CellPerRow { get => cellPerRow; set => cellPerRow = value; }
    public int CellSize { get => cellSize; set => cellSize = value; }
    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }

    private void InitializeCells(Vector2 gridCenterWoldPos)
    {
        for (int i = 0; i < CellPerRow; i++)
        {
            for (int j = 0; j < CellPerCol; j++)
            {
                Vector2Int temp = new Vector2Int(i, j);
                Cells[i, j] = new Cell(false, temp, gridCenterWoldPos, CellSize, gridWidth, gridHeight);

            }

        }

    }

    public Cell FindNextUnoccupiedCell(Vector2Int startCellID)
    {
      
        for (int y = startCellID.y; y < cellPerCol; y++)
        {
            int xStart = (y == startCellID.y) ? startCellID.x : 0;
            for (int x = xStart; x < cellPerRow; x++)
            {
                if (!cells[x, y].IsOccupied)
                {

                    return cells[x, y];
                }
            }
        }


        return null;
    }

}
