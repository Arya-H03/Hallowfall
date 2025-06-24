using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PropsBlock : MonoBehaviour
{
    [SerializeField] GameObject propsHolder;
    //[SerializeField] GameObject propsFrame;
    private ZoneLayoutProfile zoneLayoutProfile;
    public List<Transform> children = new List<Transform>();

    [SerializeField] protected Cell[,] zoneCells;
    protected int cellSize = 1;
    protected int cellsX;
    protected int cellsY;
    public  int s;

    public ZoneLayoutProfile ZoneLayoutProfile {set => zoneLayoutProfile = value; }

    protected virtual void Awake()
    {
       
       
    }
    protected virtual void Start()
    {
        InitializeCells();
        PopulateCells();
        PopulateZone(zoneCells, zoneLayoutProfile);
    }

    private void InitializeCells()
    {
        cellsX = Mathf.FloorToInt(transform.GetChild(0).localScale.x / cellSize);
        cellsY = Mathf.FloorToInt(transform.GetChild(0).localScale.y / cellSize);
        zoneCells = new Cell[cellsX, cellsY];
    }

    private void PopulateCells()
    {
        for (int i = 0; i < cellsX; i++)
        {
            for (int j = 0; j < cellsY; j++)
            {
                Vector2Int temp = new Vector2Int(i, j);
                zoneCells[i, j] = new Cell(false, temp, this.transform.position + new Vector3(cellsX /2 , cellsY/2,0), cellSize, cellsX, cellsY);

            }

        }

    }
  
    public virtual void PopulateZone(Cell[,] cells, ZoneLayoutProfile zoneLayoutProfile)
    {
        
        int cellsHeight = cells.GetLength(1);
        int cellsWidth = cells.GetLength(0);
        float propsDensity = 0.75f;

        List<Cell> randomCellsList = new List<Cell>();
        int listSize = Mathf.CeilToInt(cellsHeight * propsDensity) * Mathf.CeilToInt(cellsWidth * propsDensity);
        s = listSize;
        while (randomCellsList.Count < listSize)
        {
            int randYindex = Random.Range(0, cellsHeight);
            int randXindex = Random.Range(0, cellsWidth);
            if (!randomCellsList.Contains(cells[randXindex, randYindex]))
            {
                randomCellsList.Add(cells[randXindex, randYindex]);
            }
        }

        foreach (Cell cell in randomCellsList)
        {
            int maxAttempts = 20;
            int attempts = 0;

            while (!cell.isOccupied && attempts < maxAttempts)
            {
                attempts++;
                GameObject propPrefab = zoneLayoutProfile.GetRandomProps();
                //Color propsColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                Bounds bounds = ProceduralUtils.GetCombinedBounds(propPrefab);
                Vector3 propsScale = new Vector3(bounds.size.x, bounds.size.y + 1,1);

                // If top-right corner of the area is within zone bounds
                if (cell.cellID.x + propsScale.x <= cellsWidth && cell.cellID.y + propsScale.y <= cellsHeight)
                {
                    bool areaFree = true;

                    // Check all cells in the area (and their neighbors)
                    for (int a = cell.cellID.x; a < cell.cellID.x + propsScale.x; a++)
                    {
                        for (int b = cell.cellID.y; b < cell.cellID.y + propsScale.y; b++)
                        {
                            if (cells[a, b].isOccupied /*|| cells[a, b].CheckIfCardinalNeighboorsAreOccupied(cells)*/)
                            {
                                areaFree = false;
                            }
                        }
                    }

                    // Place object and mark area as occupied
                    if (areaFree)
                    {
                        GameObject prop = Instantiate(propPrefab, cell.cellPos + new Vector2(propsScale.x / 2, 0), Quaternion.identity);
                        prop.transform.parent = propsHolder.transform;
                        //prop.transform.localScale = propsScale;
                        //prop.transform.GetComponent<SpriteRenderer>().color = propsColor;

                        for (int i = cell.cellID.x; i < cell.cellID.x + propsScale.x; i++)
                        {
                            for (int j = cell.cellID.y; j < cell.cellID.y + propsScale.y; j++)
                            {
                                if (i >= 0 && i < cellsWidth && j >= 0 && j < cellsHeight)
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
