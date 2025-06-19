using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public bool isOccupied = false;
    public Vector2Int cellID = Vector2Int.zero;
    public Vector2 cellPos = Vector2.zero;

    public Cell(bool isOccupied, Vector2Int cellID, Vector2 zoneCenterPos, int cellSize)
    {
        this.isOccupied = isOccupied;
        this.cellID = cellID;
        this.cellPos = zoneCenterPos + new Vector2(-20, -20) + new Vector2(cellID.x * cellSize, cellID.y * cellSize);
    }

    public bool AreNeighboorsOccupied(Cell[,] cells)
    {

        Vector2Int[] allDirections = new Vector2Int[]
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
         };

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
public class MapZone : MonoBehaviour
{
    [SerializeField] private ZoneData zoneData;
    [SerializeField] private ZoneLayoutProfile zoneLayoutProfile;

    private ZoneConfig zoneConfig;

    private int cellSize = 1;
    private int cellsX;
    private int cellsY;
    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneConfig ZoneConfig { set => zoneConfig = value; }

    [SerializeField] private Cell[,] cells;

    [SerializeField] GameObject [] propPrefabs;
    private void Awake()
    {
        cellsX = Mathf.FloorToInt(40 / cellSize);
        cellsY = Mathf.FloorToInt(40 / cellSize);
        cells = new Cell[cellsX, cellsY];

       

    }
    private void Start()
    {
        for (int i = 0; i < cellsX; i++)
        {
            for (int j = 0; j < cellsY; j++)
            {
                Vector2Int temp = new Vector2Int(i, j);
                cells[i, j] = new Cell(false, temp, zoneData.centerPos, cellSize);

            }
        }

        PopulateZone();
       
    }

    public void CreateZoneEnvironemnt()
    {
        switch (zoneData.zoneType)
        {
            case ZoneType.plain:
                break;
            case ZoneType.graveYard:
                CreateFenceTilemap();
                break;
            case ZoneType.forest:
                CreateForestTilemap();
                break;

        }



    }
    private void CreateFenceTilemap()
    {
        ZoneExpansionDir dir = zoneData.previousZoneDir;
        switch (dir)
        {
            case ZoneExpansionDir.None:
                break;
            case ZoneExpansionDir.Left:
                GenerateTilemap(zoneConfig.fenceWestOpen);
                break;
            case ZoneExpansionDir.Right:
                GenerateTilemap(zoneConfig.fenceEastOpen);
                break;
            case ZoneExpansionDir.Down:
                GenerateTilemap(zoneConfig.fenceSouthOpen);
                break;
            case ZoneExpansionDir.Up:
                GenerateTilemap(zoneConfig.fenceNorthOpen);
                break;
        }
    }

    private void CreateForestTilemap()
    {
        ZoneExpansionDir dir = zoneData.previousZoneDir;
        switch (dir)
        {
            case ZoneExpansionDir.None:
                break;
            case ZoneExpansionDir.Left:
                GenerateTilemap(zoneConfig.forestWestOpen);
                break;
            case ZoneExpansionDir.Right:
                GenerateTilemap(zoneConfig.forestEastOpen);
                break;
            case ZoneExpansionDir.Down:
                GenerateTilemap(zoneConfig.forestSouthOpen);
                break;
            case ZoneExpansionDir.Up:
                GenerateTilemap(zoneConfig.forestNorthOpen);
                break;
        }
    }

    private void GenerateTilemap(GameObject[] objs)
    {
        GameObject tilemap = Instantiate(objs[Random.Range(0, objs.Length)], transform.position, Quaternion.identity);
        tilemap.transform.parent = this.transform;
    }

    public void PopulateZone()
    {
        for (int y = 1; y < cellsY - 1; y++)
        {
            for (int x = 1; x < cellsX - 1; x++)
            {
                if (Random.value > zoneLayoutProfile.clutterDensity)
                                continue;
                    // If the origin cell and its neighbors aren't occupied
                    if (!cells[x, y].isOccupied && !cells[x, y].AreNeighboorsOccupied(cells))
                {
                    Cell cell = cells[x, y];
                    GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Length)];   
                    // Setup prefab appearance
                    SpriteRenderer sr = propPrefab.GetComponent<SpriteRenderer>();
                    //propPrefab.transform.localScale = new Vector3(Random.Range(1, 10), Random.Range(1, 10), 0);

                    if (sr == null)
                    {
                        Debug.LogWarning("No SpriteRenderer found on " + propPrefab.name);
                    }

                    //// Assign random color
                    //sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);

                    // Calculate how many cells the object visually occupies
                    Bounds bounds = sr.bounds;
                    int width = Mathf.CeilToInt(bounds.size.x / cellSize);
                    int height = Mathf.CeilToInt(bounds.size.y / cellSize);

                    // If top-right corner of the area is within zone bounds
                    if (x + width < cells.GetLength(0) && y + height < cells.GetLength(1))
                    {
                        bool areaFree = true;

                        // Check all cells in the area (and their neighbors)
                        for (int a = x; a < x + width; a++)
                        {
                            for (int b = y; b < y + height; b++)
                            {
                                if (cells[a, b].isOccupied || cells[a, b].AreNeighboorsOccupied(cells))
                                {
                                    areaFree = false;
                                }
                            }
                        }

                        // Place object and mark area as occupied
                        if (areaFree)
                        {
                            GameObject prop = Instantiate(propPrefab, cell.cellPos, Quaternion.identity);
                            prop.transform.parent = this.transform;

                            for (int i = cell.cellID.x; i < cell.cellID.x + width; i++)
                            {
                                for (int j = cell.cellID.y; j < cell.cellID.y + height; j++)
                                {
                                    if (i >= 0 && i < cellsX && j >= 0 && j < cellsY)
                                    {
                                        cells[i, j].isOccupied = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }













    //for (int i = 1; i < cellsX - 1; i++)
    //{
    //    for (int j = 1; j < cellsY - 1; j++)
    //    {
    //        if (Random.value > zoneData.zoneProfile.clutterDensity)
    //            continue;

    //        Vector2 cellCenter = zoneData.centerPos + new Vector2(-20, -20) + new Vector2(i * cellSize + cellSize / 2f, j * cellSize + cellSize / 2f);
    //        Vector2 offset = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
    //        Vector2 spawnPos = cellCenter + offset;

    //        if (!Physics2D.OverlapCircle(spawnPos, 0.5f, zoneData.zoneProfile.propsMask))
    //        {
    //            GameObject prefab = zoneData.zoneProfile.GetRandomProps();
    //            if (prefab != null)
    //            {
    //                GameObject props = Instantiate(prefab, spawnPos, Quaternion.identity);
    //                props.transform.parent = this.transform;
    //            }

    //        }

    //    }
    //}
}


