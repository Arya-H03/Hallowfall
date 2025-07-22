using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystemDebugger : MonoBehaviour
{
 
    private static GridSystemDebugger instance;
    public static GridSystemDebugger Instance => instance;

   
    private Dictionary<Vector2Int, GameObject[,]> zoneVisualsDict = new();

  
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
        GameObject[,] visuals = new GameObject[cellGrid.CellPerRow, cellGrid.CellPerCol];

        cellGrid.LoopOverGrid((i, j) =>
        {
            visuals[i, j] = Instantiate(cellVisualPrefab, (Vector3Int)cellGrid.Cells[j, i].GlobalCellPos, Quaternion.identity);
        });

        zoneVisualsDict.Add(zoneCenterCoord, visuals);
    }


    public void VisualizeCellFlowDirection(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    {
        if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
        {
            InitZoneVisual(zoneCenterCoord, cellGrid);
        }

        GameObject[,] visuals = zoneVisualsDict[zoneCenterCoord];

        cellGrid.LoopOverGrid((i, j) =>
        {
            UpdateVisualForFlowDirection(cellGrid.Cells[j, i], visuals[i, j]);
        });
    }

    public void VisualizeOccupiedCells(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    {
        if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
        {
            InitZoneVisual(zoneCenterCoord, cellGrid);
        }

        GameObject[,] visuals = zoneVisualsDict[zoneCenterCoord];

        cellGrid.LoopOverGrid((i, j) =>
        {
            SpriteRenderer sr = visuals[i, j].GetComponent<SpriteRenderer>();
            sr.color = cellGrid.Cells[j, i].IsOccupied ? Color.red : Color.green;
        });
    }

    public void VisualizeWalkableCells(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    {
        if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
        {
            InitZoneVisual(zoneCenterCoord, cellGrid);
        }

        GameObject[,] visuals = zoneVisualsDict[zoneCenterCoord];

        cellGrid.LoopOverGrid((i, j) =>
        {
            SpriteRenderer sr = visuals[i, j].GetComponent<SpriteRenderer>();
            sr.color = cellGrid.Cells[j, i].IsWalkable ? Color.green : Color.red;
        });
    }

    public void VisualizeCellFlowCosts(CellGrid cellGrid, Vector2Int zoneCenterCoord)
    {
        if (!zoneVisualsDict.ContainsKey(zoneCenterCoord))
        {
            InitZoneVisual(zoneCenterCoord, cellGrid);
        }

        GameObject[,] visuals = zoneVisualsDict[zoneCenterCoord];

        cellGrid.LoopOverGrid((i, j) =>
        {
            var cell = cellGrid.Cells[j, i];
            GameObject visual = visuals[i, j];
            SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
            TextMeshProUGUI textComponent = sr.GetComponentInChildren<TextMeshProUGUI>(true);

            sr.color = cell.IsWalkable ? Color.green : Color.red;
            textComponent.enabled = true;
            textComponent.text = cell.FlowCost.ToString();
        });
    }

   
    private void UpdateVisualForFlowDirection(Cell cell, GameObject visual)
    {
        SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
        SpriteRenderer arrowSR = visual.transform.GetChild(0).GetComponent<SpriteRenderer>();

        sr.color = cell.IsWalkable ? Color.green : Color.red;
        if (cell.FlowCost == 0) sr.color = new Color(1f, 0.5f, 0f); // Orange

        arrowSR.sprite = GetArrowSprite(cell.FlowDir);
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
