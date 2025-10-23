using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystemDebugger : MonoBehaviour
{
 
    private static GridSystemDebugger instance;
    public static GridSystemDebugger Instance => instance;

   
    private Dictionary<Vector2Int, CellDebugger[,]> zoneVisualsDict = new();

  
    [SerializeField] private GameObject cellVisualPrefab;

    [Header("Arrow Sprites")]
    [SerializeField] private Sprite uArrow;
    [SerializeField] private Sprite dArrow;
    [SerializeField] private Sprite rArrow;
    [SerializeField] private Sprite lArrow;
    [SerializeField] private Sprite urArrow;
    [SerializeField] private Sprite ulArrow;
    [SerializeField] private Sprite drArrow;
    [SerializeField] private Sprite dlArrow;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

   

    private void InitZoneVisual(Vector2Int zoneCenterCoord, CellGrid cellGrid)
    {
        CellDebugger[,] visuals = new CellDebugger[cellGrid.CellPerRow, cellGrid.CellPerCol];

        cellGrid.LoopOverGrid((i, j) =>
        {
            visuals[i, j] = Instantiate(cellVisualPrefab, (Vector3Int)cellGrid.Cells[j, i].GlobalCellPos, Quaternion.identity).GetComponentInChildren<CellDebugger>();
            visuals[i,j].gameObject.transform.parent.parent = this.transform;
        });

        zoneVisualsDict.Add(zoneCenterCoord, visuals);
    }

    public void DisableAllVisuals()
    {
        foreach(KeyValuePair<Vector2Int, CellDebugger[,]> kvp in zoneVisualsDict)
        {
            for(int j = 0; j< kvp.Value.GetLength(1);  j++)
            {
                for (int i = 0; i < kvp.Value.GetLength(0); i++)
                {
                    kvp.Value[i,j].gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    public void EnableAllVisuals()
    {
        foreach (KeyValuePair<Vector2Int, CellDebugger[,]> kvp in zoneVisualsDict)
        {
            for (int j = 0; j < kvp.Value.GetLength(1); j++)
            {
                for (int i = 0; i < kvp.Value.GetLength(0); i++)
                {
                    kvp.Value[i, j].gameObject.transform.parent.gameObject.SetActive(true);
                }
            }
        }
    }
    public void VisualizeCellFlowDirection(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    {
        if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
        {
            InitZoneVisual(zoneCenterCoord, cellGrid);
        }

        CellDebugger[,] visuals = zoneVisualsDict[zoneCenterCoord];

        cellGrid.LoopOverGrid((i, j) =>
        {
            UpdateVisualForFlowDirection(cellGrid.Cells[j, i], visuals[i, j]);
           
          
        });
    }

    public void VisualizePatrolCellFlowDirection(CellGrid cellGrid)
    {
  
        CellDebugger[,] visuals = new CellDebugger[cellGrid.CellPerRow,cellGrid.CellPerCol];

        cellGrid.LoopOverGrid((i, j) =>
        {
            visuals[i, j] = Instantiate(cellVisualPrefab, (Vector3Int)cellGrid.Cells[i, j].GlobalCellPos, Quaternion.identity).GetComponentInChildren<CellDebugger>();
            visuals[i, j].gameObject.transform.parent.parent = this.transform;
        });


        cellGrid.LoopOverGrid((i, j) =>
        {
            UpdateVisualForFlowDirection(cellGrid.Cells[i, j], visuals[i, j]);
            //UpdateVisualForFlowDirection(cellGrid.Cells[j, i], visuals[j, i]);

        });
    }

    //public void VisualizeOccupiedCells(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    //{
    //    if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
    //    {
    //        InitZoneVisual(zoneCenterCoord, cellGrid);
    //    }

    //    GameObject[,] visuals = zoneVisualsDict[zoneCenterCoord];

    //    cellGrid.LoopOverGrid((i, j) =>
    //    {
    //        SpriteRenderer sr = visuals[i, j].GetComponent<SpriteRenderer>();
    //        sr.color = cellGrid.Cells[j, i].IsOccupied ? Color.red : Color.green;
    //    });
    //}

    private void UpdateVisualForFlowDirection(Cell cell, CellDebugger visual)
    {
        visual.UpdateIDText(cell.GlobalCellCoord);
        if (cell.IsWalkable) visual.UpdateColor(Color.limeGreen);
        else visual.UpdateColor(Color.red);

        if (cell.TotalCost == 0) visual.UpdateColor(Color.deepSkyBlue);
        if (cell.HasEnemy) visual.UpdateColor(Color.orange);

        visual.UpdateCostText(cell.TotalCost);

        Vector2 flowDir = cell.FlowVect;
    
        if (flowDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(flowDir.y, flowDir.x) * Mathf.Rad2Deg;
            visual.UpdateArrow(Quaternion.Euler(0, 0, angle));
        }
        else
        {
            visual.DisableArrow();
        }

    }


    private Sprite GetArrowSprite(DirectionEnum dir)
    {
        return dir switch
        {
            DirectionEnum.Right => rArrow,
            DirectionEnum.Left => lArrow,
            DirectionEnum.Up => uArrow,
            DirectionEnum.Down => dArrow,
            DirectionEnum.UpRight => urArrow,
            DirectionEnum.UpLeft => ulArrow,
            DirectionEnum.DownRight => drArrow,
            DirectionEnum.DownLeft => dlArrow,
            _ => null,
        };
    }
}
