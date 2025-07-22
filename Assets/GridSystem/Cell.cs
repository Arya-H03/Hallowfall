using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct CellPaint
{
    public Tilemap tilemap;
    public TileBase tileBase;
    public bool isOnGlobalTile;
}

public class Cell
{

    private CellGrid parentGrid;

    private bool isOccupied = false;
    private bool isPartitioned = false;
    private bool isWalkable = true;

    private DirectionEnum flowDir = DirectionEnum.None;
    private Vector2Int flowVect = Vector2Int.zero;
    private int flowCost = -1; //-1 for Unvisited, 0 for target, -2 for unWalkable, 25555 for Walkable

    private Vector3Int globalCellPos = Vector3Int.zero;

    private Vector2Int globalCellCoord = Vector2Int.zero;
    private Vector2Int localCellCoord = Vector2Int.zero;

    private HashSet<CellPaint> cellPaintHashSet = new();

    public bool IsOccupied { get => isOccupied;}
    public bool IsPartitioned { get => isPartitioned;}
    public bool IsWalkable { get => isWalkable;}

    public Vector2Int GlobalCellCoord => globalCellCoord;
    public Vector3Int GlobalCellPos => globalCellPos;
    public Vector2Int LocalCellCoord { get => localCellCoord; set => localCellCoord = value; }
    public HashSet<CellPaint> TilePaintsHasSet => cellPaintHashSet;
    public CellGrid ParentGrid => parentGrid;

    public Vector2Int FlowVect { get => flowVect;}
    public DirectionEnum FlowDir { get => flowDir; set { flowDir = value; flowVect = MyUtils.GetVectorFromDir(flowDir); } }

    public int FlowCost { get => flowCost; set => flowCost = value; }

    public Cell(CellGrid parentGrid, Vector2Int globalCellCoord, Vector3Int gridPos, int cellSize)
    {
        this.parentGrid = parentGrid;
        this.globalCellCoord = globalCellCoord;
        this.globalCellPos = gridPos + new Vector3Int(globalCellCoord.x * cellSize, globalCellCoord.y * cellSize, 0);
        this.LocalCellCoord = globalCellCoord;
    }

    public void MarkCellAsOccupied ()
    {
        isOccupied = true;
    }

    public void MarkCellAsPartitioned()
    {
        isPartitioned = true;
    }

    public void MarkCellAsUnWalkable()
    {
        isWalkable = false;
        flowCost = -2;
    }
    public void AddToCellPaint(CellPaint[] tilePaints)
    {
        foreach (CellPaint tilePaint in tilePaints)
        {
            cellPaintHashSet.Add(tilePaint);
        }

    }

    public void AddToCellPaint(CellPaint tilePaint)
    {
        cellPaintHashSet.Add(tilePaint);
    }

    public bool CheckIfAllNeighboorsAreOccupied(CellGrid cellGrid)
    {
        foreach (var dir in MyUtils.GetAllDirectionsVectorArray())
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
        foreach (var dir in MyUtils.GetCardinalDirectionsVectorArray())
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
        foreach (CellPaint tilePaint in cellPaintHashSet)
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
    public void RemoveCellPaints()
    {
        cellPaintHashSet.Clear();
    }
}
