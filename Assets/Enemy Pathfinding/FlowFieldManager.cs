using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FlowFieldManager : MonoBehaviour
{
    private static FlowFieldManager instance;
    public static FlowFieldManager Instance { get { return instance; } }

    private ZoneManager zoneManager;
    private FlowFieldGenerator flowFieldGenerator;
    private ZoneData currentTargetZoneData;
    private ZoneHandler currentTargetZoneHandler;

    private bool canVisualizeFlowField = false;

    private readonly HashSet<Cell> cellsOccupiedByEnemy = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        flowFieldGenerator = new FlowFieldGenerator();
        zoneManager = ZoneManager.Instance;
    }
    public Vector2 RequestNewGlobalFlowDir(Cell currentCell, Cell lastCell)
    {
        if (lastCell != currentCell)
        {
            if (cellsOccupiedByEnemy.Contains(lastCell))
            {          
                cellsOccupiedByEnemy.Remove(lastCell);
            }
        }
        if (!cellsOccupiedByEnemy.Contains(currentCell))
        {
            cellsOccupiedByEnemy.Add(currentCell);         
        }


        return currentCell.FlowVect.normalized;
    }

 
    public void UpdateGlobalFlowField(Vector3 targetPos)
    {
        ZoneData zoneData = zoneManager.FindZoneDataFromWorldPos(targetPos);
        if (zoneData == null) return;

        if (currentTargetZoneData != zoneData)
        {
            currentTargetZoneData = zoneData;
            currentTargetZoneHandler = currentTargetZoneData.ZoneHandler;

            foreach (KeyValuePair<Vector2Int, ZoneData> pair in zoneManager.GeneratedZonesDic)
            {
                if (pair.Value != currentTargetZoneData) UpdateFlowFieldFromZone(pair.Value);
            }
        }
        flowFieldGenerator.GenerateFlowFieldOnTargetZone(currentTargetZoneHandler.CellGrid, targetPos);

        if (canVisualizeFlowField) GridSystemDebugger.Instance.VisualizeCellFlowDirection(currentTargetZoneHandler.CellGrid, currentTargetZoneData.centerCoord);
    }

    public void UpdateFlowFieldFromZone(ZoneData zoneData)
    {
        if (currentTargetZoneData != null)
        {
            DirectionEnum direEnumToPlayer = MyUtils.FindDirectionEnumBetweenTwoPoints(new Vector2Int(zoneData.centerPos.x, zoneData.centerPos.y), new Vector2Int(currentTargetZoneData.centerPos.x, currentTargetZoneData.centerPos.y));
            ZoneHandler zoneHandler = zoneData.ZoneHandler;
            flowFieldGenerator.GenerateFlowFieldOnNonePlayerZone(zoneHandler.CellGrid, direEnumToPlayer);

            if (canVisualizeFlowField) GridSystemDebugger.Instance.VisualizeCellFlowDirection(zoneHandler.CellGrid, zoneData.centerCoord);
            
        }
    }
    public void ToggleCellDebuger()
    {
        if (canVisualizeFlowField)
        {
            canVisualizeFlowField = false;
            GridSystemDebugger.Instance.DisableAllVisuals();
        }
        else if (!canVisualizeFlowField)
        {
            canVisualizeFlowField = true;
            GridSystemDebugger.Instance.EnableAllVisuals();
        }
    }
   
}
