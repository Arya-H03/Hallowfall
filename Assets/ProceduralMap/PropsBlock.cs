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
    protected CellGrid parentCellGrid;


    public ZoneHandler ZoneHandler { get => zoneHandler; set => zoneHandler = value; }
   
    protected virtual void Awake()
    {

        propsHolder = transform.GetChild(2).transform.gameObject;

    }
    protected virtual void Start()
    {
 
    }

    public void OnPropsBlockInstantiated(CellGrid parentCellGrid, Vector3Int firstCellPos, ZoneLayoutProfile zoneLayoutProfile)
    {
        
        blockWidth = Mathf.FloorToInt(transform.GetChild(0).localScale.x);
        blockHeight = Mathf.FloorToInt(transform.GetChild(0).localScale.y);


        this.parentCellGrid = parentCellGrid;
        this.zoneLayoutProfile = zoneLayoutProfile;
        celLGrid = new CellGrid(blockWidth, blockHeight, (Vector2Int)firstCellPos, parentCellGrid);
        PopulateBlock(celLGrid, zoneLayoutProfile);
    }
    protected void VisualizeGridCells(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
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

    protected virtual void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        
       
    }
}
