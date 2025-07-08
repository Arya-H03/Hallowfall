
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TilePaint
{
    public Tilemap tilemap;
    public TileBase tileBase;
}
[System.Serializable]
public class Cell
{
   
    private bool isOccupied = false;
    private Vector2Int cellID = Vector2Int.zero;
    private Vector2Int cellPos = Vector2Int.zero;
    //private bool isPainted = false;
    List<TilePaint> tilePaintsList = new List<TilePaint>();

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public Vector2Int CellID { get => cellID; set => cellID = value; }
    public Vector2Int CellPos { get => cellPos; set => cellPos = value; }
    public List<TilePaint> TilePaintsList { get => tilePaintsList; }

    public Cell(bool isOccupied, Vector2Int cellID, Vector2Int gridPos, int cellSize)
    {
        this.isOccupied = isOccupied;
        this.cellID = cellID;
        this.cellPos = gridPos + new Vector2Int(cellID.x * cellSize, cellID.y * cellSize);
    }
    public Cell() { }

    public void AddToTilePaints(TilePaint[] tilePaints)
    {
        foreach (TilePaint tilePaint in tilePaints)
        {
            tilePaintsList.Add(tilePaint);
        }

    }

    public void AddToTilePaints(TilePaint tilePaint)
    {
        tilePaintsList.Add(tilePaint);
    }

    public bool CheckIfAllNeighboorsAreOccupied(CellGrid cellGrid)
    {

        Vector2Int[] allDirections = MyUtils.GetAllDirections();


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

        Vector2Int[] allDirections = MyUtils.GetCardinalDirections();



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

    public void PaintCell()
    {
        if (tilePaintsList.Count > 0)
        {
            foreach (TilePaint tilePaint in tilePaintsList)
            {
                tilePaint.tilemap.SetTile((Vector3Int)cellPos, tilePaint.tileBase);

            }
        }



    }

    public void RemoveTilePaint()
    {
        tilePaintsList.Clear();
    }
}
public class CellGrid
{
    private int cellSize = 1;
    private int gridWidth = 1;
    private int gridHeight = 1;

    private int cellPerRow = 1;
    private int cellPerCol = 1;

    private Cell[,] cells = null;

    private CellGrid parentCellGrid = null;


    public CellGrid(int cellSize, int gridWidth, int gridHeight, Vector2Int gridCenterWoldPos)
    {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        this.cellPerRow = Mathf.FloorToInt(this.GridWidth / cellSize);
        this.cellPerCol = Mathf.FloorToInt(this.GridHeight / cellSize);

        cells = new Cell[CellPerRow, CellPerCol];
        InitializeGridCells(gridCenterWoldPos - new Vector2Int(gridWidth / 2, gridHeight / 2));


    }

    public CellGrid(int gridWidth, int gridHeight, Vector2Int originCellWorldPos, CellGrid parentCellGrid)
    {
        this.cellSize = parentCellGrid.cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        this.cellPerRow = Mathf.FloorToInt(this.GridWidth / cellSize);
        this.cellPerCol = Mathf.FloorToInt(this.GridHeight / cellSize);


        Vector2Int originCellCoord = (originCellWorldPos - parentCellGrid.Cells[0, 0].CellPos) / cellSize;
       
        cells = new Cell[CellPerRow, CellPerCol];
        InitializeSubGridCells(parentCellGrid, parentCellGrid.Cells[originCellCoord.x, originCellCoord.y]);


    }

    public Cell[,] Cells { get => cells; set => cells = value; }
    public int CellPerCol { get => cellPerCol; set => cellPerCol = value; }
    public int CellPerRow { get => cellPerRow; set => cellPerRow = value; }
    public int CellSize { get => cellSize; set => cellSize = value; }
    public int GridWidth { get => gridWidth; set => gridWidth = value; }
    public int GridHeight { get => gridHeight; set => gridHeight = value; }
    public CellGrid ParentCellGrid { get => parentCellGrid; set => parentCellGrid = value; }

    private void InitializeGridCells(Vector2Int gridWoldPos)
    {
        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {

                Vector2Int temp = new Vector2Int(i, j);
                cells[i, j] = new Cell(false, temp, gridWoldPos, cellSize);

            }

        }
    }

    private void InitializeSubGridCells(CellGrid parentCellGrid, Cell startCell)
    {
        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {

                cells[i, j] = parentCellGrid.cells[i + startCell.CellID.x, j + startCell.CellID.y];

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

    public Cell GetCenterCellOfTheGrid()
    {

        return cells[Mathf.CeilToInt(cellPerRow / 2), Mathf.CeilToInt(cellPerCol / 2)];
    }

    public void PaintAllCells()
    {
        for (int j = 0; j < cellPerCol; j++)
        {
            for (int i = 0; i < cellPerRow; i++)
            {
                Cells[i, j].PaintCell();
            }
        }
    }

}

