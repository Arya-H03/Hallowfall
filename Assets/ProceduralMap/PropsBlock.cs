using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum PropsBlockTypeEnum
{
    GraveCluster,
    TreeCluster
}
public class PropsBlock : MonoBehaviour
{
    protected GameObject propsHolder;
    protected ZoneLayoutProfile zoneLayoutProfile;
    protected ZoneHandler zoneHandler;

    protected int cellSize = 1;
    protected int blockWidth = 1;
    protected int blockHeight = 1;

    protected CellGrid celLGrid;
    private Tilemap groundTilemap;


    public ZoneLayoutProfile ZoneLayoutProfile {set => zoneLayoutProfile = value; }
    public Tilemap GroundTilemap { get => groundTilemap; set => groundTilemap = value; }
    public ZoneHandler ZoneHandler { get => zoneHandler; set => zoneHandler = value; }

    protected virtual void Awake()
    {

        propsHolder = transform.GetChild(2).transform.gameObject;

    }
    protected virtual void Start()
    {
        blockWidth = Mathf.FloorToInt(transform.GetChild(0).localScale.x);
        blockHeight = Mathf.FloorToInt(transform.GetChild(0).localScale.y);
        celLGrid = new CellGrid(cellSize, blockWidth, blockHeight, this.transform.position + new Vector3(blockWidth /2, blockHeight/2, 0));
       
        PopulateBlock(celLGrid, zoneLayoutProfile);
    }

    protected void VisualizeGridCells(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        for (int i = 0; i < cellGrid.CellPerCol; i++)
        {
            for (int j = 0; j < cellGrid.CellPerRow; j++)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, cellGrid.Cells[j, i].CellPos, Quaternion.identity);

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

    protected virtual void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        
        //float propsDensity = 0.6f;

        //List<Cell> randomCellsList = new List<Cell>();
        //int listSize = Mathf.CeilToInt(cellGrid.CellPerCol * propsDensity) * Mathf.CeilToInt(celLGrid.CellPerRow * propsDensity);

        //while (randomCellsList.Count < listSize)
        //{
        //    int randYindex = Random.Range(0, cellGrid.CellPerCol);
        //    int randXindex = Random.Range(0, celLGrid.CellPerRow);
        //    if (!randomCellsList.Contains(cellGrid.Cells[randXindex, randYindex]))
        //    {
        //        randomCellsList.Add(cellGrid.Cells[randXindex, randYindex]);
        //    }
        //}

        //foreach (Cell cell in randomCellsList)
        //{
        //    int maxAttempts = 20;
        //    int attempts = 0;

        //    while (!cell.IsOccupied && attempts < maxAttempts)
        //    {
        //        attempts++;
        //        GameObject propPrefab = zoneLayoutProfile.GetRandomProps();
        //        //Color propsColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        //        Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
        //        Vector3 propsScale = new Vector3(bounds.size.x, bounds.size.y + 1,1);

        //        // If top-right corner of the area is within zone bounds
        //        if (cell.CellID.x + propsScale.x <= cellGrid.CellPerRow && cell.CellID.y + propsScale.y <= cellGrid.CellPerCol)
        //        {
        //            bool areaFree = true;

        //            // Check all celLGrid in the area (and their neighbors)
        //            for (int a = cell.CellID.x; a < cell.CellID.x + propsScale.x; a++)
        //            {
        //                for (int b = cell.CellID.y; b < cell.CellID.y + propsScale.y; b++)
        //                {
        //                    if (cellGrid.Cells[a, b].IsOccupied /*|| celLGrid[a, b].CheckIfCardinalNeighboorsAreOccupied(celLGrid)*/)
        //                    {
        //                        areaFree = false;
        //                    }
        //                }
        //            }

        //            // Place object and mark area as occupied
        //            if (areaFree)
        //            {
        //                GameObject prop = Instantiate(propPrefab, cell.CellPos + new Vector2(propsScale.x / 2, 0), Quaternion.identity);
        //                prop.transform.parent = propsHolder.transform;
        //                //prop.transform.localScale = propsScale;
        //                //prop.transform.GetComponent<SpriteRenderer>().color = propsColor;

        //                for (int i = cell.CellID.x; i < cell.CellID.x + propsScale.x; i++)
        //                {
        //                    for (int j = cell.CellID.y; j < cell.CellID.y + propsScale.y; j++)
        //                    {
        //                        if (i >= 0 && i < cellGrid.CellPerRow && j >= 0 && j < cellGrid.CellPerCol)
        //                        {
        //                            cellGrid.Cells[i, j].IsOccupied = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    }
        //}

    }
}
