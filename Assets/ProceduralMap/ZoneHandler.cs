using NUnit.Framework;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class Cell
{
    public bool isOccupied = false;
    public Vector2Int cellID = Vector2Int.zero;
    public Vector2 cellPos = Vector2.zero;

    public Cell(bool isOccupied, Vector2Int cellID, Vector2 zoneCenterPos, int cellSize,int cellsX,int cellsY)
    {
        this.isOccupied = isOccupied;
        this.cellID = cellID;
        this.cellPos = zoneCenterPos + new Vector2(-cellsX / 2, -cellsY/2) + new Vector2(cellID.x * cellSize, cellID.y * cellSize);
    }
    public Cell() { }
    public bool CheckIfAllNeighboorsAreOccupied(Cell[,] cells)
    {

        Vector2Int[] allDirections = ProceduralUtils.GetAllDirections();

        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = cellID + dir;
            if (neighboorID.x >= 0 && neighboorID.x < width && neighboorID.y >= 0 && neighboorID.y < height)
            {
                Cell neighboor = cells[neighboorID.x, neighboorID.y];
                if (neighboor.isOccupied) return true;
            }
        }

        return false;
    }
    public bool CheckIfCardinalNeighboorsAreOccupied(Cell[,] cells)
    {

        Vector2Int[] allDirections = ProceduralUtils.GetCardinalDirections();

        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        foreach (var dir in allDirections)
        {
            Vector2Int neighboorID = cellID + dir;
            if (neighboorID.x >= 0 && neighboorID.x < width && neighboorID.y >= 0 && neighboorID.y < height)
            {
                Cell neighboor = cells[neighboorID.x, neighboorID.y];
                if (neighboor.isOccupied) return true;
            }
        }

        return false;
    }

}
public class ZoneHandler : MonoBehaviour
{
    protected ZoneConfig zoneConfig;
    protected ZoneData zoneData;
    protected ZoneLayoutProfile zoneLayoutProfile;

    [SerializeField] protected Cell[,] zoneCells;
    protected int cellSize = 1;
    protected int cellsX;
    protected int cellsY;

    protected List<BoundsInt> listOfSubzoneBounds = new List<BoundsInt> ();
    protected List<BoundsInt> listOfPartitionedSubzoneBounds = new List<BoundsInt> ();
    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneConfig ZoneConfig { set => zoneConfig = value; }
    public ZoneLayoutProfile ZoneLayoutProfile { get => zoneLayoutProfile; set => zoneLayoutProfile = value; }

    
    protected virtual void Awake()
    {
        InitializeCells();
    }
    protected virtual void Start()
    {
        PopulateCells();
    }

    private void InitializeCells()
    {
        cellsX = Mathf.FloorToInt(40 / cellSize);
        cellsY = Mathf.FloorToInt(40 / cellSize);
        zoneCells = new Cell[cellsX, cellsY];
    }

    private void PopulateCells()
    {
        for (int i = 0; i < cellsX; i++)
        {
            for (int j = 0; j < cellsY; j++)
            {
                Vector2Int temp = new Vector2Int(i, j);
                zoneCells[i, j] = new Cell(false, temp, zoneData.centerPos, cellSize,40,40);

            }

        }

    }

    private void VisualizeGridCells(Cell[,] cells, ZoneLayoutProfile zoneLayoutProfile)
    {
        int cellsHeight = cells.GetLength(1);
        int cellsWidth = cells.GetLength(0);

        

        for (int i = 0; i < cellsHeight; i++)
        {
            for (int j = 0; j < cellsWidth; j++)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, cells[j, i].cellPos, Quaternion.identity);

                if (!cells[j, i].isOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
                }
                else if (cells[j, i].isOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
                }
            }
        }
    }

   
    public virtual void PopulateZoneWithPropBlocks(Cell[,] cells,ZoneLayoutProfile zoneLayoutProfile)
    {

        Cell startCell = FindNextUnoccupiedCell(cells,new Vector2Int(0,0));

        CreateSubZone(cells, startCell);
        listOfPartitionedSubzoneBounds = ProceduralUtils.PerformeBinarySpacePartitioning(listOfSubzoneBounds, 10, 8);

        InstantiatePropsBlocks(listOfPartitionedSubzoneBounds, zoneLayoutProfile);

    }

    private Cell FindNextUnoccupiedCell(Cell[,] cells, Vector2Int startCellID)
    {
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        for (int y = startCellID.y; y < height; y++)
        {
            int xStart = (y == startCellID.y) ? startCellID.x : 0;
            for (int x = xStart; x < width; x++)
            {
                if (!cells[x, y].isOccupied)
                {

                    return cells[x, y];
                }
            }
        }


        return null;
    }


    private void CreateSubZone(Cell[,] cells, Cell startCell)
    {
        int maxX = cells.GetLength(0);
        int maxY = cells.GetLength(1);

        int h1 = 1;
        for (int i = 1; i + startCell.cellID.y < maxY; i++)
        {
            if (cells[startCell.cellID.x, startCell.cellID.y + i].isOccupied)
                break;
            h1++;
        }

        int w1 = 1;
        for (int i = 1; i + startCell.cellID.x < maxX; i++)
        {
            if (cells[startCell.cellID.x + i, startCell.cellID.y].isOccupied)
                break;
            w1++;
        }

        int w2 = 1;
        for (int i = 1; i + startCell.cellID.x < maxX; i++)
        {
            if (cells[startCell.cellID.x + i, startCell.cellID.y + h1 - 1].isOccupied)
                break;
            w2++;
        }

        int width = Mathf.Min(w1, w2);

        int h2 = 1;
        for (int i = 1; i + startCell.cellID.y < maxY; i++)
        {
            if (cells[startCell.cellID.x + width - 1, startCell.cellID.y + i].isOccupied)
                break;
            h2++;
        }

        int height = Mathf.Min(h1, h2);

        listOfSubzoneBounds.Add(new BoundsInt(new Vector3Int((int)startCell.cellPos.x, (int)startCell.cellPos.y, 0), new Vector3Int(width, height, 0)));

        // Mark cells as occupied
        for (int x = startCell.cellID.x; x < startCell.cellID.x + width; x++)
        {
            for (int y = startCell.cellID.y; y < startCell.cellID.y + height; y++)
            {
                if (x >= 0 && x < maxX && y >= 0 && y < maxY)
                    cells[x, y].isOccupied = true;
            }
        }

        // Recursively continue
        Cell nextStartCell = FindNextUnoccupiedCell(cells, new Vector2Int(startCell.cellID.x + width, startCell.cellID.y));
        if (nextStartCell != null)
        {
            CreateSubZone(cells, nextStartCell);
        }
    }

    private void InstantiatePropsBlocks(List<BoundsInt> listOfPartitionedSubzoneBounds, ZoneLayoutProfile zoneLayoutProfile)
    {
        foreach (BoundsInt zone in listOfPartitionedSubzoneBounds)
        {
            // Instantiate zone block
            GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zone.position, Quaternion.identity);
            go.GetComponent<PropsBlock>().ZoneLayoutProfile = zoneLayoutProfile;
            go.transform.parent = this.transform;
            go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 0.2f);
            go.transform.GetChild(0).localScale = new Vector3(zone.size.x, zone.size.y, zone.size.z);
        }
    }
    protected Tilemap CreateTilemap(GameObject tilemapPrefab, Transform parent)
    {
        GameObject tilemap = Instantiate(tilemapPrefab, parent.position, Quaternion.identity);
        tilemap.transform.parent = parent;
        return tilemap.GetComponent<Tilemap>();
    }

    protected void DrawStraightLineOfTiles(Vector2Int from, Vector2Int to, RuleTile ruleTile, Tilemap tilemap)
    {
        Vector2Int delta = to - from;

        if (delta.x == 0) // Vertical road
        {
            int yMin = Mathf.Min(from.y, to.y);
            int yMax = Mathf.Max(from.y, to.y);

            for (int y = yMin; y < yMax + 1; y++)
            {
                tilemap.SetTile(PositionFromGridCell(from.x, y), ruleTile);
                zoneCells[from.x, y].isOccupied = true;
            }
        }
        else if (delta.y == 0) // Horizontal road
        {
            int xMin = Mathf.Min(from.x, to.x);
            int xMax = Mathf.Max(from.x, to.x);

            for (int x = xMin; x < xMax + 1; x++)
            {
                tilemap.SetTile(PositionFromGridCell(x, from.y), ruleTile);
                zoneCells[x, from.y].isOccupied = true;
            }
        }
    }

    protected Vector3Int PositionFromGridCell(int x, int y)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x - 20),
            Mathf.FloorToInt(y - 20),
            0
        );
    }

}


