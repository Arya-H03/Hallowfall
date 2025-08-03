using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct CellPaint
{
    public Tilemap tilemap;
    public TileBase tileBase;
    public bool isOnGlobalTile;

    public CellPaint(Tilemap tilemap, TileBase tileBase, bool isOnGlobalTile)
    {
        this.tilemap = tilemap;
        this.tileBase = tileBase;
        this.isOnGlobalTile = isOnGlobalTile;
    }
}

public enum CellFlowCost
{
    unVisited = -1,
    unWalkable = 255,
    hasEnemy = 125,
    target = 0,
}

public struct CellFlowData
{
    public bool isWalkable;
    public bool hasEnemy;

    public DirectionEnum flowDirection;
    public Vector2 flowVector;

    public int baseCost;
    public int dynamicCost;
    public int totalCost => baseCost + dynamicCost;

    public CellFlowData(
        bool isWalkable,
        bool hasEnemy,
        DirectionEnum flowDirection,
        Vector2 flowVector,
        int baseCost,
        int dynamicCost
    )
    {
        this.isWalkable = isWalkable;
        this.hasEnemy = hasEnemy;
        this.flowDirection = flowDirection;
        this.flowVector = flowVector;
        this.baseCost = baseCost;
        this.dynamicCost = dynamicCost;
    }
}


public class Cell
{
    private CellGrid parentGrid;
    private CellFlowData cellFlowData;

    private bool isOccupied = false;
    private bool isPartitioned = false;

    private float cellSize;
    private Vector3Int globalCellPos = Vector3Int.zero;
    private Vector2Int globalCellCoord = Vector2Int.zero;
    private Vector2Int localCellCoord = Vector2Int.zero;

    private HashSet<CellPaint> cellPaintHashSet = new();

    public bool IsOccupied => isOccupied;
    public bool IsPartitioned => isPartitioned;
    public bool IsWalkable => cellFlowData.isWalkable;

    public float CellSize => cellSize;
    public Vector2Int GlobalCellCoord => globalCellCoord;
    public Vector3Int GlobalCellPos => globalCellPos;
    public Vector2Int LocalCellCoord { get => localCellCoord; set => localCellCoord = value; }
    public HashSet<CellPaint> TilePaints => cellPaintHashSet;
    public CellGrid ParentGrid => parentGrid;

    public Vector2 FlowVect { get => cellFlowData.flowVector; set => cellFlowData.flowVector = value; }
    public DirectionEnum FlowDir
    {
        get => cellFlowData.flowDirection;
        set 
        {
            cellFlowData.flowDirection = value;
            cellFlowData.flowVector = MyUtils.GetVectorFromDir(value);
        }   
    }

    public int TotalCost => cellFlowData.totalCost;
    public int DynamicCost { get => cellFlowData.dynamicCost; set => cellFlowData.dynamicCost = value; }
    public int BaseCost { get => cellFlowData.baseCost; set => cellFlowData.baseCost = value; }
    public bool HasEnemy => cellFlowData.hasEnemy;

    public Cell(CellGrid parentGrid, Vector2Int globalCellCoord, Vector3Int gridPos, int cellSize)
    {
        this.parentGrid = parentGrid;
        this.globalCellCoord = globalCellCoord;
        this.cellSize = cellSize;
        this.globalCellPos = gridPos + new Vector3Int(globalCellCoord.x * cellSize, globalCellCoord.y * cellSize, 0);
        this.LocalCellCoord = globalCellCoord;
        this.cellFlowData = new CellFlowData(true, false, DirectionEnum.None, Vector2.zero, 1, 0);
    }

    public void MarkAsOccupied() => isOccupied = true;
    public void MarkAsUnoccupied() => isOccupied = false;
    public void MarkAsPartitioned() => isPartitioned = true;

    public void MarkAsUnwalkable() => cellFlowData.isWalkable = false;
    public void MarkAsWalkable() => cellFlowData.isWalkable = true;

    public void MarkOccupiedByEnemy()
    {
        if (!cellFlowData.hasEnemy)
        {
            cellFlowData.hasEnemy = true;
            cellFlowData.dynamicCost += (int)CellFlowCost.hasEnemy;
        }
       
    }

    public void MarkClearByEnemy()
    {
        if(cellFlowData.hasEnemy)
        {
            cellFlowData.hasEnemy = false;
            cellFlowData.dynamicCost -= (int)CellFlowCost.hasEnemy;
        }
       
    }

    public void AddToCellPaint(CellPaint[] tilePaints)
    {
        foreach (var tilePaint in tilePaints)
            cellPaintHashSet.Add(tilePaint);
    }

    public void AddToCellPaint(CellPaint tilePaint)
    {
        cellPaintHashSet.Add(tilePaint);
    }

    
    public bool HasOccupiedNeighbor(CellGrid cellGrid)
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

    
    public bool HasOccupiedCardinalNeighbor(CellGrid cellGrid)
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
        foreach (var tilePaint in cellPaintHashSet)
        {
            if (!tilePaint.isOnGlobalTile)
            {
                Vector3Int offset = new Vector3Int(parentGrid.CellPerRow / 2, parentGrid.CellPerCol / 2, 0);
                tilePaint.tilemap.SetTile((Vector3Int)GlobalCellCoord - offset, tilePaint.tileBase);
            }
            else
            {
                tilePaint.tilemap.SetTile(globalCellPos, tilePaint.tileBase);
            }
        }
    }

   
    public void PaintCell(Tilemap tilemap, TileBase tileBase, CellGrid parentGrid)
    {
        Vector3Int offset = new Vector3Int(parentGrid.CellPerRow / 2, parentGrid.CellPerCol / 2, 0);
        Vector3Int tilePos = (Vector3Int)GlobalCellCoord - offset;
        tilemap.SetTile(tilePos, tileBase);
    }

    public void RemoveCellPaints()
    {
        cellPaintHashSet.Clear();
    }

    public List<Cell> GetAllNeighborCells()
    {
        List<Cell> result = new();
   
        foreach (var vect in MyUtils.GetAllDirectionsVectorList())
        {
            Vector2Int neighborCoord = globalCellCoord + vect;
            if (MyUtils.IsWithinArrayBounds(parentGrid.Cells, neighborCoord))
                result.Add(parentGrid.Cells[neighborCoord.x, neighborCoord.y]);
        }

        return result;
    }

    public List<Cell> GetCardinalNeighborCells()
    {
        List<Cell> result = new();
      

        foreach (var vect in MyUtils.GetCardinalDirectionsVectorList())
        {
            Vector2Int neighborCoord = globalCellCoord + vect;
            if (MyUtils.IsWithinArrayBounds(parentGrid.Cells, neighborCoord))
                result.Add(parentGrid.Cells[neighborCoord.x, neighborCoord.y]);
        }

        return result;
    }
}
