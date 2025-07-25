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
    private Cell currentTargetCell;
    private bool canVisualizeFlowField = false;
    private List<Cell> cellsOccupiedByEnemy = new List<Cell>();
  
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
    public Vector3 RequestNewPosition(Vector3 callerCurrentPos, Vector3 callerLastPos)
    {
        ZoneData zoneData = zoneManager.FindZoneDateFromorldPos(callerCurrentPos);

        Cell lastCell = zoneManager.FindCurrentCellFromWorldPos(callerLastPos);
        lastCell.MarkClearByEnemy();
        cellsOccupiedByEnemy.Remove(lastCell);

       
        Cell currentCell = zoneData.ZoneHandler.CellGrid.GetCellFromWorldPos(callerCurrentPos);
        cellsOccupiedByEnemy.Add(currentCell);
        currentCell.MarkOccupiedByEnemy();
       

        Vector3 flowVector = new Vector3(currentCell.FlowVect.x, currentCell.FlowVect.y, 0);
        Vector3 newPos = flowVector + currentCell.GlobalCellPos + new Vector3(currentCell.CellSize / 2, currentCell.CellSize / 2, 0);

        

        return newPos;
    }

  
    public void UpdateFlowFieldFromTarget(Vector3 targetPos)
    {
        ZoneData zoneData = zoneManager.FindZoneDateFromorldPos(targetPos);
       
        if (currentTargetZoneData != zoneData)
        {
            currentTargetZoneData = zoneData;
            currentTargetZoneHandler = currentTargetZoneData.ZoneHandler;

            foreach (KeyValuePair<Vector2Int, ZoneData> pair in ZoneManager.Instance.GeneratedZonesDic)
            {

                if (pair.Value != currentTargetZoneData) UpdateFlowFieldFromZone(pair.Value);
            }
        }
        flowFieldGenerator.GenerateFlowFieldOnTargetZone(currentTargetZoneHandler.CellGrid, targetPos);

        Cell targetCell = currentTargetZoneHandler.CellGrid.GetCellFromWorldPos((Vector3)targetPos);

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
   
    //public bool ValidateTargetCellHasChanged(Vector3 targetPos)
    //{
    //    ZoneData zoneData = zoneManager.FindZoneDateFromorldPos(targetPos);
    //    if (currentTargetZoneData != zoneData)
    //    {
    //        currentTargetZoneData = zoneData;
    //        //foreach (KeyValuePair<Vector2Int, ZoneData> pair in zoneManager.GeneratedZonesDic)
    //        //{

    //        //    if (pair.Value != currentTargetZoneData) RequestFlowFieldGenerationOnNonePlayerGrid(pair.Value);
    //        //}
    //    }
    //    CellGrid cellGrid = currentTargetZoneData.ZoneHandler.CellGrid;
    //    Cell cell = cellGrid.GetCellFromWorldPos(targetPos);

    //    if (currentTargetCell != cell)
    //    {
    //        currentTargetCell = cell;
    //        return true;
    //    }
    //    else { return true; }


    //}
}
