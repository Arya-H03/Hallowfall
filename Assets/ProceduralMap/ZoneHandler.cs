
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;



public class ZoneHandler : MonoBehaviour
{
   

    protected ZoneData zoneData;
    protected ZoneLayoutProfile zoneLayoutProfile;

    protected int cellSize;
    protected int zoneWidth;
    protected int zoneHeight;

    protected CellGrid celLGrid;

    protected List<BoundsInt> listOfSubzoneBounds = new List<BoundsInt>();
    protected List<BoundsInt> listOfPartitionedSubzoneBounds = new List<BoundsInt>();


    [SerializeField] protected Tilemap groundZeroTilemap;
    [SerializeField] protected Tilemap groundOneTilemap;
    [SerializeField] protected Tilemap groundTwoTilemap;

    [SerializeField] protected Tilemap boundsTilemap;
    [SerializeField] protected Tilemap propsTilemap;
    [SerializeField] protected Tilemap treeTilemap;


    public Tilemap GroundZeroTilemap { get => groundZeroTilemap; }
    public Tilemap GroundOneTilemap { get => groundOneTilemap; }
    public Tilemap GroundTwoTilemap { get => groundTwoTilemap; }
    public Tilemap PropsTilemap { get => propsTilemap; }
    public Tilemap BoundsTilemap { get => boundsTilemap; }
    public Tilemap TreeTilemap { get => treeTilemap; }
   

    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneLayoutProfile ZoneLayoutProfile { get => zoneLayoutProfile; set => zoneLayoutProfile = value; }


    private void OnValidate()
    {
      


        MyUtils.ValidateFields(this, propsTilemap, nameof(propsTilemap));
        MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));
        MyUtils.ValidateFields(this, groundOneTilemap, nameof(groundZeroTilemap));
        MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));

        MyUtils.ValidateFields(this, boundsTilemap, nameof(boundsTilemap));
        MyUtils.ValidateFields(this, TreeTilemap, nameof(TreeTilemap));
    }


    protected virtual void Awake()
    {
      
    }

    public virtual void Init(ZoneData zoneData, ZoneLayoutProfile zoneLayoutProfile,int zoneWidth, int zoneHeight, int cellSize)
    {
        this.zoneData = zoneData;
        this.zoneLayoutProfile = zoneLayoutProfile;
        this.zoneWidth = zoneWidth;
        this.zoneHeight = zoneHeight;
        this.cellSize = cellSize;

        celLGrid = new CellGrid(cellSize, zoneWidth, zoneHeight, zoneData.centerPos);
        StartCoroutine(GenerateZoneCoroutine());

    }
    protected virtual IEnumerator GenerateZoneCoroutine()
    {
        return null;
    }
    private void VisualizeGridCells(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        for (int i = 0; i < cellGrid.CellPerCol; i++)
        {
            for (int j = 0; j < cellGrid.CellPerRow; j++)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, (Vector3Int)cellGrid.Cells[j, i].GlobalCellPos, Quaternion.identity);

                if (!cellGrid.Cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
                }
                else if (cellGrid.Cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
                }
            }
        }
    }


    public virtual void PopulateZoneWithPropBlocks(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {

        Cell startCell = cellGrid.FindNextUnpartitionedCell(new Vector2Int(0, 0));

        CreateSubZone(cellGrid, startCell);
        listOfPartitionedSubzoneBounds = MyUtils.PerformeBinarySpacePartitioning(listOfSubzoneBounds, 8, 8);

        InstantiatePropsBlocks(listOfPartitionedSubzoneBounds, zoneLayoutProfile);

    }


    private void CreateSubZone(CellGrid cellGrid, Cell startCell)
    {

        int h1 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x, startCell.GlobalCellCoord.y + i].IsPartitioned)
                break;
            h1++;
        }

        int w1 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + i, startCell.GlobalCellCoord.y].IsPartitioned)
                break;
            w1++;
        }

        int w2 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + i, startCell.GlobalCellCoord.y + h1 - 1].IsPartitioned)
                break;
            w2++;
        }

        int width = Mathf.Min(w1, w2);

        int h2 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + width - 1, startCell.GlobalCellCoord.y + i].IsPartitioned)
                break;
            h2++;
        }

        int height = Mathf.Min(h1, h2);

        listOfSubzoneBounds.Add(new BoundsInt(new Vector3Int((int)startCell.GlobalCellPos.x, (int)startCell.GlobalCellPos.y, 0), new Vector3Int(width, height, 0)));

        // Mark cellGrid as occupied
        for (int x = startCell.GlobalCellCoord.x; x < startCell.GlobalCellCoord.x + width; x++)
        {
            for (int y = startCell.GlobalCellCoord.y; y < startCell.GlobalCellCoord.y + height; y++)
            {
                if (x >= 0 && x < cellGrid.CellPerRow && y >= 0 && y < cellGrid.CellPerCol)
                    cellGrid.Cells[x, y].IsPartitioned = true;
            }
        }

        // Recursively continue
        Cell nextStartCell = cellGrid.FindNextUnpartitionedCell(new Vector2Int(startCell.GlobalCellCoord.x + width, startCell.GlobalCellCoord.y));
        if (nextStartCell != null)
        {
            CreateSubZone(cellGrid, nextStartCell);
        }
    }

    private void InstantiatePropsBlocks(List<BoundsInt> listOfPartitionedSubzoneBounds, ZoneLayoutProfile zoneLayoutProfile)
    {
        foreach (BoundsInt zoneBounds in listOfPartitionedSubzoneBounds)
        {
            

            Type componentType = GetPropsBlockType(zoneLayoutProfile, zoneBounds);
            if (componentType != null)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zoneBounds.position, Quaternion.identity);
                go.transform.parent = this.transform;
                Component addedComponent = go.AddComponent(componentType);
                PropsBlock propsBlock = addedComponent as PropsBlock;

                if (propsBlock)
                {
                    propsBlock.Init(this,celLGrid, zoneBounds.position, zoneLayoutProfile,zoneBounds);
                }
                else
                {
                    Destroy(go);
                }
            }


        }


    }

    private Type GetPropsBlockType(ZoneLayoutProfile zoneLayoutProfile, BoundsInt blockBounds)
    {
        Type type = null;

        if(zoneLayoutProfile.propsBlocksStructList.Count <1)
        {
            return null;
        }
        while (type == null)
        {
            PropsBlockStruct propsBlock = zoneLayoutProfile.propsBlocksStructList[Random.Range(0, zoneLayoutProfile.propsBlocksStructList.Count)];
            if (blockBounds.size.x >= propsBlock.minBlockSize.x && blockBounds.size.y >= propsBlock.minBlockSize.y)
            {
                type = propsBlock.scriptReference.GetClass();
            }

        }


        return type;
    }
    protected Tilemap CreateTilemap(GameObject tilemapPrefab, Transform parent)
    {
        GameObject tilemap = Instantiate(tilemapPrefab, parent.position, Quaternion.identity);
        tilemap.transform.parent = parent;
        return tilemap.GetComponent<Tilemap>();
    }

    protected virtual void AddDefaultGroundTileForZone(ZoneLayoutProfile zoneLayoutProfile)
    {

        TilePaint tilePaint = new TilePaint {/* tilemap = ZoneManager.Instance.GroundZeroTilemap*/ tilemap = this.GroundZeroTilemap, tileBase = zoneLayoutProfile.defaultGroundTile };

        for (int j = 0; j < celLGrid.CellPerCol; j++)
        {
            for(int i  = 0; i < celLGrid.CellPerRow; i++)
            {
                celLGrid.Cells[i,j].AddToTilePaints(tilePaint);
            }
        }
    }

    protected void DrawStraightLineOfTiles(Vector2Int beginningCellCoord, Vector2Int endCellCoord, TilePaint[] tilePaints)
    {
        Vector2Int delta = endCellCoord - beginningCellCoord;

        if (delta.x == 0) // Vertical road
        {
            
            int yMin = Mathf.Min(beginningCellCoord.y, endCellCoord.y);
            int yMax = Mathf.Max(beginningCellCoord.y, endCellCoord.y);

            for (int y = yMin; y < yMax + 1; y++)
            {
                Vector3Int pos = TurnCellCoordToTilePos(beginningCellCoord.x, y);
                //tilemap.SetTile(pos, tileBase);
                celLGrid.Cells[beginningCellCoord.x, y].IsOccupied = true;
                celLGrid.Cells[beginningCellCoord.x, y].IsPartitioned = true;
                celLGrid.Cells[beginningCellCoord.x, y].AddToTilePaints(tilePaints);
            }
        }
        else if (delta.y == 0) // Horizontal road
        {
            int xMin = Mathf.Min(beginningCellCoord.x, endCellCoord.x);
            int xMax = Mathf.Max(beginningCellCoord.x, endCellCoord.x);

            for (int x = xMin; x < xMax + 1; x++)
            {

                Vector3Int pos = TurnCellCoordToTilePos(x, beginningCellCoord.y);
                //tilemap.SetTile(pos, tileBase);
                celLGrid.Cells[x, beginningCellCoord.y].IsOccupied = true;
                celLGrid.Cells[x, beginningCellCoord.y].IsPartitioned = true;
                celLGrid.Cells[x, beginningCellCoord.y].AddToTilePaints(tilePaints);
            }
        }
    }

    protected Vector3Int TurnCellCoordToTilePos(int x, int y)
    {
        return (Vector3Int)celLGrid.Cells[x, y].GlobalCellPos;
    }
 

}


