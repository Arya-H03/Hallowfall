using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlowFieldGenerator
{
    private readonly List<Vector2Int> dirVects = MyUtils.GetAllDirectionsVectorList();

    public void GenerateFlowFieldOnPlayerzone(CellGrid cellGrid, Vector3 targetPos)
    {
        Queue<Cell> flowFieldCellQueue = new Queue<Cell>();
        List<Cell>unWalkableCells = new List<Cell>();
        List<Cell>unWalkableNeighborCells = new List<Cell>();
        Cell targetCell = FindTargetCell(cellGrid, targetPos, flowFieldCellQueue);

        SetInitialFlowCosts(cellGrid, targetCell, unWalkableCells);

        BFS(cellGrid, flowFieldCellQueue);

        foreach (Cell cell in unWalkableCells)
        {
            foreach (Cell neighborCell in cell.GetAllNeighborCells())
            {

                if (neighborCell.IsWalkable && neighborCell != targetCell)
                {


                    neighborCell.FlowCost += 1;
                    unWalkableNeighborCells.Add(neighborCell);
                }
            }
        }

        AssignFlowDirections(cellGrid, targetCell);

    }

    public void GenerateFlowFieldOnNonePlayerZone(CellGrid cellGrid, DirectionEnum dirToPlayerZone)
    {
        Queue<Cell> flowFieldCellQueue = new Queue<Cell>();
        List<Cell> unWalkableCells = new List<Cell>();
        List<Cell> unWalkableNeighborCells = new List<Cell>();
        FindAllEdgeCells(cellGrid, flowFieldCellQueue, dirToPlayerZone, unWalkableCells);

        BFS(cellGrid, flowFieldCellQueue);
        foreach (Cell cell in unWalkableCells)
        {
            foreach (Cell neighborCell in cell.GetAllNeighborCells())
            {
               
                if (neighborCell.IsWalkable)
                {
                   
                    neighborCell.FlowCost += 2;
                    unWalkableNeighborCells.Add(neighborCell);
                }
            }
        }
        //AssignFlowDirections(cellGrid);
    }

    private Cell FindTargetCell(CellGrid cellGrid, Vector3 targetPos, Queue<Cell> flowFieldCellQueue)
    {
       
        Cell targetCell = cellGrid.GetCellFromWorldPos(targetPos);
        flowFieldCellQueue.Enqueue(targetCell);
        return targetCell;

    }

    private void FindAllEdgeCells(CellGrid cellGrid, Queue<Cell> flowFieldCellQueue, DirectionEnum dirToPlayerZone, List<Cell> unWalkableCells)
    {

        cellGrid.LoopOverGrid((i, j) =>
        {
            Cell currentCell = cellGrid.Cells[i, j];
            if (!currentCell.IsWalkable)
            {
                currentCell.FlowCost = (int)CellCosts.unWalkable;
                unWalkableCells.Add(currentCell);
            } 
            else currentCell.FlowCost = (int)CellCosts.unVisited;
        });


        switch (dirToPlayerZone)
        {
            case DirectionEnum.Right:
                EnqueueRightEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Left:
                EnqueueLeftEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Up:
                EnqueueTopEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Down:
                EnqueueBottomEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.UpRight:
                EnqueueTopEdge(cellGrid, flowFieldCellQueue);
                EnqueueRightEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.UpLeft:
                EnqueueTopEdge(cellGrid, flowFieldCellQueue);
                EnqueueLeftEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.DownRight:
                EnqueueBottomEdge(cellGrid, flowFieldCellQueue);
                EnqueueRightEdge(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.DownLeft:
                EnqueueBottomEdge(cellGrid, flowFieldCellQueue);
                EnqueueLeftEdge(cellGrid, flowFieldCellQueue);
                break;
        }
    }


    private void SetInitialFlowCosts(CellGrid cellGrid, Cell targetCell,List<Cell>unWalkableCells)
    {

        for (int j = 0; j < cellGrid.CellPerCol; j++)
        {
            for (int i = 0; i < cellGrid.CellPerRow; i++)
            {
                Cell currentCell = cellGrid.Cells[i, j];
                if (currentCell == targetCell) currentCell.FlowCost = (int)CellCosts.target;
                else if (!currentCell.IsWalkable)
                {
                    currentCell.FlowCost = (int)CellCosts.unWalkable;
                    unWalkableCells.Add(currentCell);
                }
                else currentCell.FlowCost = (int)CellCosts.unVisited;
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
            bool isWithinBounds = MyUtils.IsWithinArrayBounds(cellGrid.Cells, neighborCoord);

            if (!isWithinBounds) continue;

            Cell neighborCell = cellGrid.Cells[neighborCoord.x, neighborCoord.y];

            bool isWalkable = neighborCell.FlowCost != (int)CellCosts.unWalkable;
            bool isVisited = neighborCell.FlowCost != (int)CellCosts.unVisited;

            if (isWalkable && !isVisited) result.Add(neighborCell);


        }

        return result;
    }

    private void AssignFlowDirections(CellGrid cellGrid,Cell targetCell)
    {
        for (int j = 0; j < cellGrid.CellPerCol; j++)
        {
            for (int i = 0; i < cellGrid.CellPerRow; i++)
            {
                cellGrid.Cells[i, j].FlowVect = FindFlowVector(cellGrid, cellGrid.Cells[i, j], targetCell);
            }
        }
    }

    private Vector2 FindFlowVector(CellGrid cellGrid, Cell currentCell, Cell targetCell)
    {
        if (!currentCell.IsWalkable || currentCell.FlowCost < (int)CellCosts.target)
            return Vector2.zero;

        Vector2 accumulatedFlow = Vector2.zero;
        float totalWeight = 0f;

        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (!neighbor.IsWalkable || neighbor.FlowCost < (int)CellCosts.target) continue;

            float costDiff = currentCell.FlowCost - neighbor.FlowCost;
            if (costDiff > 0)
            {
                Vector2 dirVect = new Vector2(
                    (neighbor.GlobalCellPos - currentCell.GlobalCellPos).x,
                    (neighbor.GlobalCellPos - currentCell.GlobalCellPos).y
                );
                accumulatedFlow += dirVect.normalized * costDiff;
                totalWeight += costDiff;
            }
        }

        if (totalWeight > 0f)
        {
            Vector2 blendedFlow = accumulatedFlow.normalized;

            // Snap to the closest walkable neighbor direction
            Cell bestMatch = null;
            float bestDot = -Mathf.Infinity;

            foreach (Cell neighbor in currentCell.GetAllNeighborCells())
            {
                if (!neighbor.IsWalkable) continue;

                Vector2 dirVect = new Vector2(
                    (neighbor.GlobalCellPos - currentCell.GlobalCellPos).x,
                    (neighbor.GlobalCellPos - currentCell.GlobalCellPos).y
                );
                Vector2 toNeighbor = dirVect.normalized;
                float dot = Vector2.Dot(blendedFlow, toNeighbor);

                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestMatch = neighbor;
                }
            }

            if (bestMatch != null)
            {
                return new Vector2(
                    (bestMatch.GlobalCellPos - currentCell.GlobalCellPos).x,
                    (bestMatch.GlobalCellPos - currentCell.GlobalCellPos).y
                ).normalized;
            }
        }

        // Fallback: pick cheapest valid neighbor
        Cell cheapestNeighbor = null;
        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (!neighbor.IsWalkable || neighbor.FlowCost < (int)CellCosts.target) continue;

            if (cheapestNeighbor == null || neighbor.FlowCost < cheapestNeighbor.FlowCost)
            {
                cheapestNeighbor = neighbor;
            }
            else if (neighbor.FlowCost == cheapestNeighbor.FlowCost)
            {
                float dist1 = (neighbor.GlobalCellPos - targetCell.GlobalCellPos).sqrMagnitude;
                float dist2 = (cheapestNeighbor.GlobalCellPos - targetCell.GlobalCellPos).sqrMagnitude;

                if (dist1 < dist2)
                    cheapestNeighbor = neighbor;
            }
        }

        if (cheapestNeighbor != null)
        {
            return new Vector2(
                (cheapestNeighbor.GlobalCellPos - currentCell.GlobalCellPos).x,
                (cheapestNeighbor.GlobalCellPos - currentCell.GlobalCellPos).y
            ).normalized;
        }

        return Vector2.zero;
    }


    private DirectionEnum FindFlowDirection(CellGrid cellGrid, Cell currentCell)
    {

        if (!currentCell.IsWalkable || currentCell.FlowCost < (int)CellCosts.target) return DirectionEnum.None;

        Cell cheapestNeighbor = null;
        foreach (Vector2Int dirVect in dirVects)
        {
            Vector2Int neighborCoord = new Vector2Int(currentCell.GlobalCellCoord.x + dirVect.x, currentCell.GlobalCellCoord.y + dirVect.y);
            bool isWithinBounds = MyUtils.IsWithinArrayBounds(cellGrid.Cells, neighborCoord);
            if (!isWithinBounds) continue;

            Cell neighborCell = cellGrid.Cells[neighborCoord.x, neighborCoord.y];

            if (!neighborCell.IsWalkable || neighborCell.FlowCost < (int)CellCosts.target) continue;

            if (cheapestNeighbor == null || neighborCell.FlowCost < cheapestNeighbor.FlowCost) cheapestNeighbor = neighborCell;

        }

        if (cheapestNeighbor == null) return DirectionEnum.None;

        return MyUtils.GetDirFromVector(cheapestNeighbor.GlobalCellCoord - currentCell.GlobalCellCoord);
    }

    private void EnqueueRightEdge(CellGrid grid, Queue<Cell> queue)
    {
        int x = grid.CellPerRow - 1;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            TryEnqueue(grid.Cells[x, y], queue);
        }
    }

    private void EnqueueLeftEdge(CellGrid grid, Queue<Cell> queue)
    {
        int x = 0;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            TryEnqueue(grid.Cells[x, y], queue);
        }
    }

    private void EnqueueTopEdge(CellGrid grid, Queue<Cell> queue)
    {
        int y = grid.CellPerCol - 1;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            TryEnqueue(grid.Cells[x, y], queue);
        }
    }

    private void EnqueueBottomEdge(CellGrid grid, Queue<Cell> queue)
    {
        int y = 0;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            TryEnqueue(grid.Cells[x, y], queue);
        }
    }

    private void TryEnqueue(Cell cell, Queue<Cell> queue)
    {
        if (!cell.IsWalkable) return;

        cell.FlowCost = 0;
        queue.Enqueue(cell);
    }

}
