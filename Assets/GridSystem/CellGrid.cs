using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct BlockPaint
{
    public Tilemap tilemap;
    public TileBase tileBase;
    public BoundsInt blockBounds;
}
public class CellGrid
{
    protected int cellSize;
    protected int gridWidth;
    protected int gridHeight;

    protected int cellPerRow;
    protected int cellPerCol;

    protected Cell[,] cells;
    protected List<BlockPaint> blockPaintList = new();

    public Cell[,] Cells => cells;
    public int CellPerCol => cellPerCol;
    public int CellPerRow => cellPerRow;
    public int CellSize => cellSize;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public List<BlockPaint> BlockPaintList => blockPaintList;

    public CellGrid(int cellSize, int gridWidth, int gridHeight)
    {
        this.cellSize = cellSize;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        cellPerRow = Mathf.FloorToInt(gridWidth / cellSize);
        cellPerCol = Mathf.FloorToInt(gridHeight / cellSize);

        cells = new Cell[cellPerRow, cellPerCol];
    }

    public void InitializeGridCells(Vector3Int gridCenterWorldPos)
    {
        Vector3Int originWorldPos = gridCenterWorldPos - new Vector3Int(gridWidth / 2, gridHeight / 2, 0);

        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {
                Vector2Int cellCoord = new(i, j);
                cells[i, j] = new Cell(this, cellCoord, originWorldPos, cellSize);
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

    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        Vector3Int cellPosOnGrid = new Vector3Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), 0);
        Vector3Int cellCoord = cellPosOnGrid - cells[0,0].GlobalCellPos;
        return cells[cellCoord.x, cellCoord.y];
    }

    public Cell GetCenterCellOfGrid() =>
        cells[cellPerRow / 2, cellPerCol / 2];

    public void AddToBlockPaints(Tilemap tilemap, TileBase tileBase, SubCellGrid cellGrid, CellGrid parentGrid)
    {
        BoundsInt blockBounds = new BoundsInt(
            (Vector3Int)cellGrid.Cells[0, 0].GlobalCellCoord - new Vector3Int(parentGrid.CellPerRow / 2, parentGrid.CellPerCol / 2, 0),
            new Vector3Int(cellGrid.CellPerRow, cellGrid.CellPerCol, 1)
        );

        BlockPaint blockPaint = new BlockPaint
        {
            tilemap = tilemap,
            tileBase = tileBase,
            blockBounds = blockBounds
        };

        parentGrid.BlockPaintList.Add(blockPaint);
    }

    private void PaintBlock(BlockPaint blockPaint)
    {
        if (blockPaint.tileBase == null || blockPaint.tilemap == null)
            return;

        BoundsInt bounds = blockPaint.blockBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        TileBase[] tiles = new TileBase[width * height];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = blockPaint.tileBase;

        blockPaint.tilemap.SetTilesBlock(bounds, tiles);
    }

    public IEnumerator PaintAllBlocksCoroutine()
    {
        int count = 0;
        foreach (var block in BlockPaintList)
        {
            PaintBlock(block);
            count++;
            if (count >= 5)
            {
                count = 0;
                yield return null;
                
            }
        } 
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
                if (count >= 200)
                {
                    count = 0;
                    yield return null;
                }
            }
        }
    }

    public void PaintAllCells()
    {
        LoopOverGrid((i, j) => cells[i, j].PaintCell(this));
    }

    public GameObject TryInstantiatePermanentGameobjectOnTile(GameObject prefab, Vector2Int localCellCoord, Quaternion rotation, bool markOccupied, Transform parent = null)
    {
        if (!MyUtils.IsWithinArrayBounds(cellPerRow, cellPerCol, localCellCoord)) return null;

        Cell cell = cells[localCellCoord.x, localCellCoord.y];
        if (cell.IsOccupied) return null;

        Vector3 pos = (Vector3)cell.GlobalCellPos + new Vector3(0.5f, 0.5f, 0);
        GameObject go = UnityEngine.Object.Instantiate(prefab, pos, rotation);
        if (parent != null) go.transform.parent = parent;
        if (markOccupied) cell.MarkAsOccupied();

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

    //public bool IsCoordWithinBounds(Vector2Int coord)
    //{
    //    return coord.x >= 0 && coord.y >= 0 && coord.x < cellPerRow && coord.y < cellPerCol;
    //}

}