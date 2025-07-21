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

    private Vector3Int globalCellPos = Vector3Int.zero;

    private Vector2Int globalCellCoord = Vector2Int.zero;
    private Vector2Int localCellCoord = Vector2Int.zero;

    private HashSet<CellPaint> cellPaintHashSet = new();

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsPartitioned { get => isPartitioned; set => isPartitioned = value; }
    public Vector2Int GlobalCellCoord => globalCellCoord;
    public Vector3Int GlobalCellPos => globalCellPos;
    public Vector2Int LocalCellCoord { get => localCellCoord; set => localCellCoord = value; }
    public HashSet<CellPaint> TilePaintsHasSet => cellPaintHashSet;
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
