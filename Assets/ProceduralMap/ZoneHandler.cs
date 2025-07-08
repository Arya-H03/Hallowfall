
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;



public class ZoneHandler : MonoBehaviour
{
    protected Tilemap groundTilemap;

    protected ZoneConfig zoneConfig;
    protected ZoneData zoneData;
    protected ZoneLayoutProfile zoneLayoutProfile;

    protected int cellSize = 1;
    protected int zoneWidth = 40;
    protected int zoneHeight = 40;

    protected CellGrid celLGrid;

    protected List<BoundsInt> listOfSubzoneBounds = new List<BoundsInt>();
    protected List<BoundsInt> listOfPartitionedSubzoneBounds = new List<BoundsInt>();
    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneConfig ZoneConfig { set => zoneConfig = value; }
    public ZoneLayoutProfile ZoneLayoutProfile { get => zoneLayoutProfile; set => zoneLayoutProfile = value; }


    protected virtual void Awake()
    {
        groundTilemap = transform.GetChild(0).GetComponentInChildren<Tilemap>();
    }
    protected virtual void Start()
    {
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
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, (Vector3Int)cellGrid.Cells[j, i].CellPos, Quaternion.identity);

                if (!cellGrid.Cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
                }
                else if (cellGrid.Cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
                }
            }
        }
    }


    public virtual void PopulateZoneWithPropBlocks(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {

        Cell startCell = cellGrid.FindNextUnoccupiedCell(new Vector2Int(0, 0));

        CreateSubZone(cellGrid, startCell);
        listOfPartitionedSubzoneBounds = MyUtils.PerformeBinarySpacePartitioning(listOfSubzoneBounds, 10, 8);

        InstantiatePropsBlocks(listOfPartitionedSubzoneBounds, zoneLayoutProfile);

    }


    private void CreateSubZone(CellGrid cellGrid, Cell startCell)
    {

        int h1 = 1;
        for (int i = 1; i + startCell.CellID.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.CellID.x, startCell.CellID.y + i].IsOccupied)
                break;
            h1++;
        }

        int w1 = 1;
        for (int i = 1; i + startCell.CellID.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.CellID.x + i, startCell.CellID.y].IsOccupied)
                break;
            w1++;
        }

        int w2 = 1;
        for (int i = 1; i + startCell.CellID.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.CellID.x + i, startCell.CellID.y + h1 - 1].IsOccupied)
                break;
            w2++;
        }

        int width = Mathf.Min(w1, w2);

        int h2 = 1;
        for (int i = 1; i + startCell.CellID.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.CellID.x + width - 1, startCell.CellID.y + i].IsOccupied)
                break;
            h2++;
        }

        int height = Mathf.Min(h1, h2);

        listOfSubzoneBounds.Add(new BoundsInt(new Vector3Int((int)startCell.CellPos.x, (int)startCell.CellPos.y, 0), new Vector3Int(width, height, 0)));

        // Mark cellGrid as occupied
        for (int x = startCell.CellID.x; x < startCell.CellID.x + width; x++)
        {
            for (int y = startCell.CellID.y; y < startCell.CellID.y + height; y++)
            {
                if (x >= 0 && x < cellGrid.CellPerRow && y >= 0 && y < cellGrid.CellPerCol)
                    cellGrid.Cells[x, y].IsOccupied = true;
            }
        }

        // Recursively continue
        Cell nextStartCell = cellGrid.FindNextUnoccupiedCell(new Vector2Int(startCell.CellID.x + width, startCell.CellID.y));
        if (nextStartCell != null)
        {
            CreateSubZone(cellGrid, nextStartCell);
        }
    }

    private void InstantiatePropsBlocks(List<BoundsInt> listOfPartitionedSubzoneBounds, ZoneLayoutProfile zoneLayoutProfile)
    {
        foreach (BoundsInt zoneBounds in listOfPartitionedSubzoneBounds)
        {
            GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zoneBounds.position, Quaternion.identity);

            Type componentType = GetPropsBlockType(zoneLayoutProfile, zoneBounds);
            if (componentType != null)
            {
                Component addedComponent = go.AddComponent(componentType);
                PropsBlock propsBlock = addedComponent as PropsBlock;

                if (propsBlock)
                {
                    
                    go.transform.parent = this.transform;
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 0f);
                    go.transform.GetChild(0).localScale = new Vector3(zoneBounds.size.x, zoneBounds.size.y, zoneBounds.size.z);

                    propsBlock.OnPropsBlockInstantiated(celLGrid,zoneBounds.position,zoneLayoutProfile);
                }
            }


        }


    }

    private Type GetPropsBlockType(ZoneLayoutProfile zoneLayoutProfile, BoundsInt blockBounds)
    {
        Type type = null;

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
                celLGrid.Cells[x, beginningCellCoord.y].AddToTilePaints(tilePaints);
            }
        }
    }

    protected Vector3Int TurnCellCoordToTilePos(int x, int y)
    {
        return (Vector3Int)celLGrid.Cells[x, y].CellPos;
    }

 
}


