
using System.Collections;
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
    private bool isPartitioned = false;

    //Used within a parentgrid
    private Vector2Int globalCellCoord = Vector2Int.zero;
    private Vector2Int globalCellPos = Vector2Int.zero;

    //Used within a subgrid
    private Vector2Int localCellCoord = Vector2Int.zero;

    HashSet<TilePaint> tilePaintsHasSet = new HashSet<TilePaint>();

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsPartitioned { get => isPartitioned; set => isPartitioned = value; }

    public Vector2Int GlobalCellCoord { get => globalCellCoord;}
    public Vector2Int GlobalCellPos { get => globalCellPos;}
    public Vector2Int LocalCellCoord { get => localCellCoord; set => localCellCoord = value; }



    public HashSet<TilePaint> TilePaintsHasSet { get => tilePaintsHasSet; }
    

    public Cell(bool isOccupied, bool isPartitioned, Vector2Int globalCellCoord, Vector2Int gridPos, int cellSize)
    {
        this.isOccupied = isOccupied;
        this.IsPartitioned = isPartitioned;
        this.globalCellCoord = globalCellCoord;
        this.globalCellPos = gridPos + new Vector2Int(globalCellCoord.x * cellSize, globalCellCoord.y * cellSize);
        this.LocalCellCoord = globalCellCoord;
    }
    public Cell() { }

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

        Vector2Int[] allDirections = MyUtils.GetAllDirectionsVector();


        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = GlobalCellCoord + dir;
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

        Vector2Int[] allDirections = MyUtils.GetCardinalDirectionsVector();



        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = GlobalCellCoord + dir;
            if (neighboorID.x >= 0 && neighboorID.x < cellGrid.CellPerRow && neighboorID.y >= 0 && neighboorID.y < cellGrid.CellPerCol)
            {
                Cell neighboor = cellGrid.Cells[neighboorID.x, neighboorID.y];
                if (neighboor.IsOccupied) return true;
            }
        }

        return false;
    }

    public void PaintCell(CellGrid parentGrid)
    {
        if (tilePaintsHasSet.Count > 0)
        {
            foreach (TilePaint tilePaint in tilePaintsHasSet)
            {
                if (!tilePaint.tileBase) Debug.Log("Tilebase" + "Tilemap:" + tilePaint.tilemap);
                if (!tilePaint.tilemap) Debug.Log("Tilemap " + "Tilebase:" + tilePaint.tileBase);
                if (parentGrid == null) Debug.Log("(parentGrid");
                tilePaint.tilemap.SetTile((Vector3Int)GlobalCellCoord - new Vector3Int(parentGrid.CellPerRow /2, parentGrid.CellPerCol/2, 0), tilePaint.tileBase);

            }
        }
    }

    public void PaintCell(Tilemap tilemap,TileBase tileBase, CellGrid parentGrid)
    {
        tilemap.SetTile((Vector3Int)GlobalCellCoord - new Vector3Int(parentGrid.CellPerRow/2, parentGrid.CellPerCol, 0)/2, tileBase);
    }

    public void RemoveTilePaint()
    {
        tilePaintsHasSet.Clear();
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


        Vector2Int originCellCoord = (originCellWorldPos - parentCellGrid.Cells[0, 0].GlobalCellPos) / cellSize;

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

                Vector2Int cellGlobalCoord = new Vector2Int(i, j);

                cells[i, j] = new Cell(false, false, cellGlobalCoord, gridWoldPos, cellSize);

            }

        }
    }

    private void InitializeSubGridCells(CellGrid parentCellGrid, Cell startCell)
    {
        for (int i = 0; i < cellPerRow; i++)
        {
            for (int j = 0; j < cellPerCol; j++)
            {

                cells[i, j] = parentCellGrid.cells[i + startCell.GlobalCellCoord.x, j + startCell.GlobalCellCoord.y];
                cells[i, j].LocalCellCoord = cells[i, j].GlobalCellCoord - new Vector2Int(startCell.GlobalCellCoord.x, startCell.GlobalCellCoord.y);

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
                {

                    return cells[x, y];
                }
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
                Cells[i, j].PaintCell(this);
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
        for (int j = 0; j < cellPerCol; j++)
        {
            for (int i = 0; i < cellPerRow; i++)
            {
                Cells[i, j].PaintCell(this);
            }
            
        }
    }
    public GameObject TryInstantiatePremanantGameobjectOnTile(GameObject prefab, Vector2Int localCelCoord, Quaternion rotation, bool shouldMakeTileOccupied, Transform parent = null)
    {
        
        if (MyUtils.IsWithinArrayBounds(cellPerRow, cellPerCol, localCelCoord))
        {
            if (!cells[localCelCoord.x, localCelCoord.y].IsOccupied)
            {
                Vector3 pos = new Vector3(cells[localCelCoord.x, localCelCoord.y].GlobalCellPos.x, cells[localCelCoord.x, localCelCoord.y].GlobalCellPos.y, 0) + new Vector3(0.5f, 0.5f, 0);
                GameObject go = Object.Instantiate(prefab, pos, rotation);
                if (parent != null) go.transform.parent = parent;

                if (shouldMakeTileOccupied) cells[localCelCoord.x, localCelCoord.y].IsOccupied = true;
                return go;
            }
            else
            {
                //Debug.Log("Failed to instantiate" + nameof(prefab) + " at " + localCelCoord + " due to cell being occupied.");
                return null;
            }

        }
        else
        {
            //Debug.Log("Failed to instantiate" + nameof(prefab) + " at " + localCelCoord + " due to being out of bounds. The grid bound is "+ cellPerRow + " / " + cellPerCol);
            return null;
        }


    }

    public GameObject TryInstantiateTempGameobjectOnTile(GameObject prefab, Vector2Int localCelCoord, Quaternion rotation, Transform parent = null)
    {
        if (MyUtils.IsWithinArrayBounds(cellPerRow, cellPerCol, localCelCoord))
        {
            Vector3 pos = new Vector3(cells[localCelCoord.x, localCelCoord.y].GlobalCellPos.x, cells[localCelCoord.x, localCelCoord.y].GlobalCellPos.y, 0) + new Vector3(0.5f, 0.5f, 0);
            GameObject go = Object.Instantiate(prefab, pos, rotation);
            if (parent != null) go.transform.parent = parent;

            return go;


        }
        else return null;


    }

    public void LoopOverGrid(System.Action<int, int> function)
    {
        for (int j = 0; j < cellPerCol; j++)
        {
            for (int i = 0; i < cellPerRow; i++)
            {
                function(i, j);
            }
        }

    }
}

