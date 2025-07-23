using System.Collections.Generic;
using UnityEngine;

public class FlowFieldGenerator
{
    private readonly List<Vector2Int> dirVects = MyUtils.GetAllDirectionsVectorList();

    public void GenerateFlowField(CellGrid cellGrid, Vector3 targetPos)
    {
        Queue<Cell> flowFieldCellQueue = new Queue<Cell>();
        Cell targetCurrentCell = FindTargetCell(cellGrid, targetPos, flowFieldCellQueue);
        SetInitialFlowCosts(cellGrid, targetCurrentCell);

        BFS(cellGrid, flowFieldCellQueue);
        AssignFlowDirections(cellGrid);
    }

    private Cell FindTargetCell(CellGrid cellGrid, Vector3 targetPos, Queue<Cell> flowFieldCellQueue)
    {
        Cell targetCell = cellGrid.GetCellFromWorldPos(targetPos);
        flowFieldCellQueue.Enqueue(targetCell);
        return targetCell;

    }

    private void SetInitialFlowCosts(CellGrid cellGrid, Cell targetCell)
    {

        for (int j = 0; j < cellGrid.CellPerCol; j++)
        {
            for (int i = 0; i < cellGrid.CellPerRow; i++)
            {
                Cell currentCell = cellGrid.Cells[i, j];
                if (currentCell == targetCell) currentCell.FlowCost = 0;
                //if (i == 0 || j == 0 || i == cellGrid.CellPerRow-1 ||i == cellGrid.CellPerRow-1) currentCell.FlowCost = 0;
                else if (!currentCell.IsWalkable) currentCell.FlowCost = -2;
                else currentCell.FlowCost = -1;
            }
        }
    }

    private void BFS(CellGrid cellGrid, Queue<Cell> flowFieldCellQueue)
    {
        while (flowFieldCellQueue.Count > 0)
        {
            Cell currentCell = flowFieldCellQueue.Dequeue();
            foreach (Cell cell in GetAllValidNeighbors(cellGrid, currentCell))
            {
                flowFieldCellQueue.Enqueue(cell);
                cell.FlowCost = currentCell.FlowCost + 1;
            }
        }
    }

    private List<Cell> GetAllValidNeighbors(CellGrid cellGrid, Cell centerCell)
    {
        List<Cell> result = new List<Cell>();
        foreach (Vector2Int dirVect in dirVects)
        {
            Vector2Int neighborCoord = new Vector2Int(centerCell.GlobalCellCoord.x + dirVect.x, centerCell.GlobalCellCoord.y + dirVect.y);

            bool isWithinBounds = cellGrid.IsCoordWithinBounds(neighborCoord);

            if (!isWithinBounds) continue;

            Cell neighborCell = cellGrid.Cells[neighborCoord.x, neighborCoord.y];

            bool isWalkable = neighborCell.FlowCost != -2;
            bool isVisited = neighborCell.FlowCost != -1;

            if (isWalkable && !isVisited) result.Add(neighborCell);
           

        }

        return result;
    }

    private void AssignFlowDirections(CellGrid cellGrid)
    {
        for (int j = 0; j < cellGrid.CellPerCol; j++)
        {
            for (int i = 0; i < cellGrid.CellPerRow; i++)
            {
                cellGrid.Cells[i, j].FlowDir = FindFlowDirection(cellGrid, cellGrid.Cells[i, j]);
            }
        }
    }

    private DirectionEnum FindFlowDirection(CellGrid cellGrid, Cell currentCell)
    {

        if (!currentCell.IsWalkable || currentCell.FlowCost <= 0) return DirectionEnum.None;

        Cell cheapestNeighbor = null;
        foreach (Vector2Int dirVect in dirVects)
        {
            Vector2Int neighborCoord = new Vector2Int(currentCell.GlobalCellCoord.x + dirVect.x, currentCell.GlobalCellCoord.y + dirVect.y);
            bool isWithinBounds = cellGrid.IsCoordWithinBounds(neighborCoord);
            if (!isWithinBounds) continue;

            Cell neighborCell = cellGrid.Cells[neighborCoord.x, neighborCoord.y];

            if (!neighborCell.IsWalkable || neighborCell.FlowCost < 0) continue;

            if (cheapestNeighbor == null || neighborCell.FlowCost < cheapestNeighbor.FlowCost) cheapestNeighbor = neighborCell;

        }

        if(cheapestNeighbor == null) return DirectionEnum.None;

        return MyUtils.GetDirFromVector(cheapestNeighbor.GlobalCellCoord - currentCell.GlobalCellCoord);
    }
}
