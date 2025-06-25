using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
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

    protected List<BoundsInt> listOfSubzoneBounds = new List<BoundsInt> ();
    protected List<BoundsInt> listOfPartitionedSubzoneBounds = new List<BoundsInt> ();
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
    }

    private void VisualizeGridCells(Cell[,] cells, ZoneLayoutProfile zoneLayoutProfile)
    {
        int cellsHeight = cells.GetLength(1);
        int cellsWidth = cells.GetLength(0);

        

        for (int i = 0; i < cellsHeight; i++)
        {
            for (int j = 0; j < cellsWidth; j++)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, cells[j, i].CellPos, Quaternion.identity);

                if (!cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
                }
                else if (cells[j, i].IsOccupied)
                {
                    go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.2f);
                }
            }
        }
    }

   
    public virtual void PopulateZoneWithPropBlocks(CellGrid cellGrid,ZoneLayoutProfile zoneLayoutProfile)
    {

        Cell startCell = cellGrid.FindNextUnoccupiedCell(new Vector2Int(0,0));

        CreateSubZone(cellGrid, startCell);
        listOfPartitionedSubzoneBounds = ProceduralUtils.PerformeBinarySpacePartitioning(listOfSubzoneBounds, 10, 8);

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
        foreach (BoundsInt zone in listOfPartitionedSubzoneBounds)
        {         
            GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zone.position, Quaternion.identity);
            
            Type componentType = zoneLayoutProfile.propsBlockClassList[Random.Range(0, zoneLayoutProfile.propsBlockClassList.Count)];
            Component addedComponent = go.AddComponent(componentType);       
            PropsBlock propsBlock = addedComponent as PropsBlock;
          
            if (propsBlock)
            {
                propsBlock.ZoneLayoutProfile = zoneLayoutProfile;
                propsBlock.GroundTilemap = groundTilemap;
                go.transform.parent = this.transform;
                go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 0);
                go.transform.GetChild(0).localScale = new Vector3(zone.size.x, zone.size.y, zone.size.z);
            }
         
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
                celLGrid.Cells[from.x, y].IsOccupied = true;
            }
        }
        else if (delta.y == 0) // Horizontal road
        {
            int xMin = Mathf.Min(from.x, to.x);
            int xMax = Mathf.Max(from.x, to.x);

            for (int x = xMin; x < xMax + 1; x++)
            {
                tilemap.SetTile(PositionFromGridCell(x, from.y), ruleTile);
                celLGrid.Cells[x, from.y].IsOccupied = true;
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


