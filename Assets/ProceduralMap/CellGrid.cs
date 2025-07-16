
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct TilePaint
{
    public Tilemap tilemap;
    public TileBase tileBase;
    public bool isOnGlobalTile;
}
[System.Serializable]
public class Cell
{

    private CellGrid parentGrid;
    private bool isOccupied = false;
    private bool isPartitioned = false;

    private Vector2Int globalCellCoord = Vector2Int.zero;
    private Vector3Int globalCellPos = Vector3Int.zero;
    private Vector2Int localCellCoord = Vector2Int.zero;

    private HashSet<TilePaint> tilePaintsHasSet = new();

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsPartitioned { get => isPartitioned; set => isPartitioned = value; }
    public Vector2Int GlobalCellCoord => globalCellCoord;
    public Vector3Int GlobalCellPos => globalCellPos;
    public Vector2Int LocalCellCoord { get => localCellCoord; set => localCellCoord = value; }
    public HashSet<TilePaint> TilePaintsHasSet => tilePaintsHasSet;
    public CellGrid ParentGrid => parentGrid;

    public Cell(CellGrid parentGrid, bool isOccupied, bool isPartitioned, Vector2Int globalCellCoord, Vector3Int gridPos, int cellSize)
    {
        this.parentGrid = parentGrid;
        this.isOccupied = isOccupied;
        this.IsPartitioned = isPartitioned;
        this.globalCellCoord = globalCellCoord;
        this.globalCellPos = gridPos + new Vector3Int(globalCellCoord.x * cellSize, globalCellCoord.y * cellSize, 0);
        this.LocalCellCoord = globalCellCoord;
    }


    public void AddToTilePaints(TilePaint[] tilePaints)
    {
        foreach (TilePaint tilePaint in tilePaints)
        {
            tilePaintsHasSet.Add(tilePaint);
        }

    }

    public void AddToTilePaints(TilePaint tilePaint)
    {
        tilePaintsHasSet.Add(tilePaint);
    }

    public bool CheckIfAllNeighboorsAreOccupied(CellGrid cellGrid)
    {
        foreach (var dir in MyUtils.GetAllDirectionsVector())
        {
            Vector2Int neighborID = GlobalCellCoord + dir;
            if (MyUtils.IsWithinArrayBounds(cellGrid.CellPerRow, cellGrid.CellPerCol, neighborID))
            {
                if (cellGrid.Cells[neighborID.x, neighborID.y].IsOccupied)
                    return true;
            }
        }
        return false;
    }

    public bool CheckIfCardinalNeighboorsAreOccupied(CellGrid cellGrid)
    {
        foreach (var dir in MyUtils.GetCardinalDirectionsVector())
        {
            Vector2Int neighborID = GlobalCellCoord + dir;
            if (MyUtils.IsWithinArrayBounds(cellGrid.CellPerRow, cellGrid.CellPerCol, neighborID))
            {
                if (cellGrid.Cells[neighborID.x, neighborID.y].IsOccupied)
                    return true;
            }
        }
        return false;
    }

    public void PaintCell(CellGrid parentGrid)
    {
        foreach (TilePaint tilePaint in tilePaintsHasSet)
        {
            if (!tilePaint.isOnGlobalTile) tilePaint.tilemap.SetTile((Vector3Int)GlobalCellCoord - new Vector3Int(parentGrid.CellPerRow / 2, parentGrid.CellPerCol / 2, 0), tilePaint.tileBase);
            else tilePaint.tilemap.SetTile((Vector3Int)globalCellPos, tilePaint.tileBase);

        }
    }

    public void PaintCell(Tilemap tilemap, TileBase tileBase, CellGrid parentGrid)
    {
        Vector3Int tilePos = (Vector3Int)GlobalCellCoord - new Vector3Int(parentGrid.CellPerRow / 2, parentGrid.CellPerCol, 0) / 2;
        tilemap.SetTile(tilePos, tileBase);
    }

    public void RemoveTilePaint()
    {
        tilePaintsHasSet.Clear();
    }
}
public class CellGrid
{
    private int cellSize;
    private int gridWidth;
    private int gridHeight;

    private int cellPerRow;
    private int cellPerCol;

    private Cell[,] cells;
    private CellGrid parentCellGrid;

    public Cell[,] Cells { get => cells; }
    public int CellPerCol { get => cellPerCol; }
    public int CellPerRow { get => cellPerRow; }
    public int CellSize { get => cellSize; }
    public int GridWidth { get => gridWidth; }
    public int GridHeight { get => gridHeight; }
    public CellGrid ParentCellGrid { get => parentCellGrid; }

    public CellGrid(int cellSize, int gridWidth, int gridHeight, Vector3Int gridCenterWorldPos)
    {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        cellPerRow = Mathf.FloorToInt(gridWidth / cellSize);
        cellPerCol = Mathf.FloorToInt(gridHeight / cellSize);

        cells = new Cell[cellPerRow, cellPerCol];
        InitializeGridCells(gridCenterWorldPos - new Vector3Int(gridWidth / 2, gridHeight / 2, 0));
    }

    public CellGrid(int gridWidth, int gridHeight, Vector3Int originCellWorldPos, CellGrid parentCellGrid)
    {
        this.parentCellGrid = parentCellGrid;
        this.cellSize = parentCellGrid.cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        cellPerRow = Mathf.FloorToInt(gridWidth / cellSize);
        cellPerCol = Mathf.FloorToInt(gridHeight / cellSize);

        Vector3Int originCellCoord = (originCellWorldPos - parentCellGrid.Cells[0, 0].GlobalCellPos) / cellSize;
        cells = new Cell[cellPerRow, cellPerCol];
        InitializeSubGridCells(parentCellGrid, parentCellGrid.Cells[originCellCoord.x, originCellCoord.y]);
    }
    private void InitializeGridCells(Vector3Int originWorldPos)
    {
        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {
                Vector2Int cellCoord = new(i, j);
                cells[i, j] = new Cell(this, false, false, cellCoord, originWorldPos, cellSize);
            }
        }
    }

    private void InitializeSubGridCells(CellGrid parent, Cell startCell)
    {
        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {
                cells[i, j] = parent.cells[i + startCell.GlobalCellCoord.x, j + startCell.GlobalCellCoord.y];
                cells[i, j].LocalCellCoord = cells[i, j].GlobalCellCoord - startCell.GlobalCellCoord;
            }
        }
    }

    public Cell FindNextUnpartitionedCell(Vector2Int startCellID)
    {
        for (int y = startCellID.y; y < cellPerCol; y++)
        {
            int xStart = (y == startCellID.y) ? startCellID.x : 0;
            for (int x = xStart; x < cellPerRow; x++)
            {
                if (!cells[x, y].IsPartitioned)
                    return cells[x, y];
            }
        }
        return null;
    }


    public Cell GetCenterCellOfGrid()
    {
        return cells[Mathf.FloorToInt(cellPerRow / 2), Mathf.FloorToInt(cellPerCol / 2)];
    }
    public IEnumerator PaintAllCellsCoroutine()
    {
        int count = 0;
        for (int j = 0; j < cellPerCol; j++)
        {
            for (int i = 0; i < cellPerRow; i++)
            {
                cells[i, j].PaintCell(this);
                count++;
                if (count >= 50)
                {
                    count = 0;
                    yield return null;
                }
            }
        }
    }

    public void PaintAllCells()
    {
        LoopOverGrid((i, j) =>
        {
            Cells[i, j].PaintCell(this);
        });

    }
    public GameObject TryInstantiatePermanentGameobjectOnTile(GameObject prefab, Vector2Int localCellCoord, Quaternion rotation, bool shouldMakeTileOccupied, Transform parent = null)
    {
        if (!MyUtils.IsWithinArrayBounds(cellPerRow, cellPerCol, localCellCoord)) return null;

        Cell cell = cells[localCellCoord.x, localCellCoord.y];
        if (cell.IsOccupied) return null;

        Vector3 pos = (Vector3)cell.GlobalCellPos + new Vector3(0.5f, 0.5f, 0);
        GameObject go = UnityEngine.Object.Instantiate(prefab, pos, rotation);
        if (parent != null) go.transform.parent = parent;
        if (shouldMakeTileOccupied) cell.IsOccupied = true;
        return go;
    }

    public GameObject TryInstantiateTempGameobjectOnTile(GameObject prefab, Vector2Int localCellCoord, Quaternion rotation, Transform parent = null)
    {
        if (!MyUtils.IsWithinArrayBounds(cellPerRow, cellPerCol, localCellCoord)) return null;

        Vector3 pos = (Vector3)cells[localCellCoord.x, localCellCoord.y].GlobalCellPos + new Vector3(0.5f, 0.5f, 0);
        GameObject go = UnityEngine.Object.Instantiate(prefab, pos, rotation);
        if (parent != null) go.transform.parent = parent;
        return go;
    }

    public void LoopOverGrid(Action<int, int> action)
    {
        for (int j = 0; j < cellPerCol; j++)
        {
            for (int i = 0; i < cellPerRow; i++)
            {
                action(i, j);
            }
        }


    }
}

