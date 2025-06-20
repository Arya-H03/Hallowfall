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
public class ZoneHandler : MonoBehaviour
{
    protected ZoneConfig zoneConfig;
    protected ZoneData zoneData;
    protected ZoneLayoutProfile zoneLayoutProfile;

    [SerializeField] protected Cell[,] cells;
    protected int cellSize = 1;
    protected int cellsX;
    protected int cellsY;
    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneConfig ZoneConfig { set => zoneConfig = value; }
    public ZoneLayoutProfile ZoneLayoutProfile { get => zoneLayoutProfile; set => zoneLayoutProfile = value; }

    
    protected virtual void Awake()
    {
        cellsX = Mathf.FloorToInt(40 / cellSize);
        cellsY = Mathf.FloorToInt(40 / cellSize);
        cells = new Cell[cellsX, cellsY];



    }
    protected virtual void Start()
    {
        for (int i = 0; i < cellsX; i++)
        {
            for (int j = 0; j < cellsY; j++)
            {
                Vector2Int temp = new Vector2Int(i, j);
                cells[i, j] = new Cell(false, temp, zoneData.centerPos, cellSize);

            }

        }

        CreateZoneEnvironemnt();

    }


    protected virtual void CreateZoneEnvironemnt()
    {
        //switch (zoneData.zoneType)
        //{
        //    case ZoneType.plain:
        //        break;
        //    case ZoneType.graveYard:
        //        CreateBoundsTilemap();
                
        //        break;
        //    //case ZoneType.forest:
        //    //    CreateForestTilemap();
        //    //    break;

        //}
    }
    protected virtual void CreateBoundsTilemap()
    {
       
    }

    //private void CreateForestTilemap()
    //{
    //    ZoneDir dir = zoneData.previousZoneDir;
    //    switch (dir)
    //    {
    //        case ZoneDir.None:
    //            break;
    //        case ZoneDir.Left:
    //            GenerateTilemap(zoneConfig.forestWestOpen);
    //            break;
    //        case ZoneDir.Right:
    //            GenerateTilemap(zoneConfig.forestEastOpen);
    //            break;
    //        case ZoneDir.Down:
    //            GenerateTilemap(zoneConfig.forestSouthOpen);
    //            break;
    //        case ZoneDir.Up:
    //            GenerateTilemap(zoneConfig.forestNorthOpen);
    //            break;
    //    }
    //}

    protected virtual void GenerateTilemap(GameObject[] objs)
    {
        GameObject tilemap = Instantiate(objs[Random.Range(0, objs.Length)], transform.position, Quaternion.identity);
        tilemap.transform.parent = this.transform;
    }

    public virtual void PopulateZone()
    {
        for (int y = 1; y < cellsY - 1; y++)
        {
            for (int x = 1; x < cellsX - 1; x++)
            {
                if (Random.value > ZoneLayoutProfile.clutterDensity)
                    continue;
               
                // If the origin cell and its neighbors aren't occupied
                if (!cells[x, y].isOccupied && !cells[x, y].AreNeighboorsOccupied(cells))
                {
                    Cell cell = cells[x, y];
                    GameObject propPrefab = zoneLayoutProfile.GetRandomProps();
                    // Setup prefab appearance
                    SpriteRenderer sr = propPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>();
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
                            prop.GetComponent<PropsBlock>().RandomizeProps();

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
}


