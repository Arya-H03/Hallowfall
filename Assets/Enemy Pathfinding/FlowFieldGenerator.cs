using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowFieldGenerator
{
    private const int COST_UNVISITED = (int)CellFlowCost.unVisited;
    private const int COST_UNWALKABLE = (int)CellFlowCost.unWalkable;
    private const int COST_TARGET = (int)CellFlowCost.target;

    public void GenerateFlowFieldOnTargetZone(CellGrid cellGrid, Vector3 targetPos)
    {
        Queue<Cell> flowFieldCellQueue = new();

        Cell currentFieldTargetCell = cellGrid.GetCellFromWorldPos(targetPos);

        SetInitialBaseFlowCosts(cellGrid, currentFieldTargetCell, flowFieldCellQueue);

        BFS(flowFieldCellQueue);

        AssignFlowAllDirectionsOnPlayerZone(cellGrid);

    }

    public void GenerateFlowFieldOnNonePlayerZone(CellGrid cellGrid, DirectionEnum dirToPlayerZone)
    {
        Queue<Cell> flowFieldCellQueue = new();

        FindAllEdgeCells(cellGrid, flowFieldCellQueue, dirToPlayerZone);

        BFS(flowFieldCellQueue);

        AssignFlowAllDirectionsOnNonePlayerZone(cellGrid, dirToPlayerZone);
    }


    private void FindAllEdgeCells(CellGrid cellGrid, Queue<Cell> flowFieldCellQueue, DirectionEnum dirToPlayerZone)
    {
        
        cellGrid.LoopOverGrid((i, j) =>
        {
            Cell currentCell = cellGrid.Cells[i, j];
            if (!currentCell.IsWalkable)
            {
                currentCell.BaseCost = COST_UNWALKABLE;
            }
            else currentCell.BaseCost = COST_UNVISITED;
        });


        switch (dirToPlayerZone)
        {
            case DirectionEnum.Right:
                FindRightEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Left:
                FindLeftEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Up:
                FindTopEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.Down:
                FindBottomEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.UpRight:
                FindTopEdgeTargetCells(cellGrid, flowFieldCellQueue);
                FindRightEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.UpLeft:
                FindTopEdgeTargetCells(cellGrid, flowFieldCellQueue);
                FindLeftEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.DownRight:
                FindBottomEdgeTargetCells(cellGrid, flowFieldCellQueue);
                FindRightEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
            case DirectionEnum.DownLeft:
                FindBottomEdgeTargetCells(cellGrid, flowFieldCellQueue);
                FindLeftEdgeTargetCells(cellGrid, flowFieldCellQueue);
                break;
        }
    }


    private void SetInitialBaseFlowCosts(CellGrid cellGrid, Cell targetCell, Queue<Cell> flowFieldCellQueue)
    {
        cellGrid.LoopOverGrid((i, j) =>
        {
            Cell currentCell = cellGrid.Cells[i, j];

            if (!currentCell.IsWalkable)
            {
                currentCell.BaseCost = COST_UNWALKABLE;
            }
            else
            {
                currentCell.BaseCost = COST_UNVISITED;

            }
        }
        );

        targetCell.BaseCost = COST_TARGET;
        flowFieldCellQueue.Enqueue(targetCell);
    }

    private void BFS(Queue<Cell> flowFieldCellQueue)
    {
       List<Cell> neighborList = new(8);

        while (flowFieldCellQueue.Count > 0)
        {
            Cell currentCell = flowFieldCellQueue.Dequeue();
            neighborList.Clear();
            neighborList.AddRange(currentCell.GetAllNeighborCells());
            foreach (Cell neighbor in neighborList)
            {
                if (neighbor.IsWalkable && neighbor.BaseCost == COST_UNVISITED)
                {
                    neighbor.BaseCost = currentCell.BaseCost + 1;
                    flowFieldCellQueue.Enqueue(neighbor);
                }

            }
        }
    }
    private void AssignFlowAllDirectionsOnPlayerZone(CellGrid cellGrid)
    {
       
        cellGrid.LoopOverGrid((i, j) =>
        {
            cellGrid.Cells[i, j].FlowDir = FindFlowForPlayerDirection(cellGrid.Cells[i, j]);            
        }
        );

    }

    private void AssignFlowAllDirectionsOnNonePlayerZone(CellGrid cellGrid, DirectionEnum directionEnum)
    {

        cellGrid.LoopOverGrid((i, j) =>
        {
            cellGrid.Cells[i, j].FlowDir = FindFlowDirectionForNonePlayerZone(cellGrid.Cells[i, j], directionEnum);
           
        }
        );
    }

    private DirectionEnum FindFlowForPlayerDirection(Cell currentCell)
    {
        if (currentCell.TotalCost == COST_TARGET) return DirectionEnum.None;
      
        List<Cell> cheapestNeighbors = new();
        int minCost = int.MaxValue;

        foreach(Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (!neighbor.IsWalkable) continue;
            if(neighbor.TotalCost < minCost)
            {
                cheapestNeighbors.Clear();
                minCost = neighbor.TotalCost;
                cheapestNeighbors.Add(neighbor);
            }
            else if(neighbor.TotalCost == minCost)
            {
                cheapestNeighbors.Add(neighbor);
            }
        }
            
        List<Cell> validNeighborsWithEnemy = cheapestNeighbors.Where(cell => !cell.HasEnemy).ToList();

        if(validNeighborsWithEnemy.Count == 0) return DirectionEnum.None;

        Cell selected = validNeighborsWithEnemy[Random.Range(0, validNeighborsWithEnemy.Count)];
        Vector2Int dir = selected.GlobalCellCoord - currentCell.GlobalCellCoord;
        return MyUtils.GetDirFromVector(dir);

    }

    private DirectionEnum FindFlowDirectionForNonePlayerZone(Cell currentCell,DirectionEnum directionEnum)
    {
        if (currentCell.TotalCost == COST_TARGET) return directionEnum;

        List<Cell> cheapestNeighbors = new();
        int minCost = int.MaxValue;

        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (!neighbor.IsWalkable) continue;
            if (neighbor.TotalCost < minCost)
            {
                cheapestNeighbors.Clear();
                minCost = neighbor.TotalCost;
                cheapestNeighbors.Add(neighbor);
            }
            else if (neighbor.TotalCost == minCost)
            {
                cheapestNeighbors.Add(neighbor);
            }
        }

        List<Cell> validNeighborsWithEnemy = cheapestNeighbors.Where(cell => !cell.HasEnemy).ToList();

        if (validNeighborsWithEnemy.Count == 0) return DirectionEnum.None;

        Cell selected = validNeighborsWithEnemy[Random.Range(0, validNeighborsWithEnemy.Count)];
        Vector2Int dir = selected.GlobalCellCoord - currentCell.GlobalCellCoord;
        return MyUtils.GetDirFromVector(dir);
    }


    private Vector2 FindFlowVector(CellGrid cellGrid, Cell currentCell, Cell targetCell)
    {
        if (/*!currentCell.IsWalkable ||*/ currentCell.TotalCost < (int)CellFlowCost.target)
            return Vector2.zero;

        Vector2 accumulatedFlow = Vector2.zero;
        float totalWeight = 0f;

        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (/*!neighbor.IsWalkable ||*/ neighbor.TotalCost < (int)CellFlowCost.target) continue;

            float costDiff = currentCell.TotalCost - neighbor.TotalCost;
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
                //if (!neighbor.IsWalkable) continue;

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
            if (/*!neighbor.IsWalkable ||*/ neighbor.TotalCost < (int)CellFlowCost.target) continue;

            if (cheapestNeighbor == null || neighbor.TotalCost < cheapestNeighbor.TotalCost)
            {
                cheapestNeighbor = neighbor;
            }
            else if (neighbor.TotalCost == cheapestNeighbor.TotalCost)
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

    private void FindRightEdgeTargetCells(CellGrid grid, Queue<Cell> flowFieldCellQueue)
    {
        int x = grid.CellPerRow - 1;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            if (!grid.Cells[x, y].IsWalkable) continue;

            grid.Cells[x, y].BaseCost = COST_TARGET;
            flowFieldCellQueue.Enqueue(grid.Cells[x, y]);
           
        }
    }

    private void FindLeftEdgeTargetCells(CellGrid grid, Queue<Cell> flowFieldCellQueue)
    {
        int x = 0;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            if (!grid.Cells[x, y].IsWalkable) continue;

            grid.Cells[x, y].BaseCost = COST_TARGET;
            flowFieldCellQueue.Enqueue(grid.Cells[x, y]);
        }
    }

    private void FindTopEdgeTargetCells(CellGrid grid, Queue<Cell> flowFieldCellQueue)
    {
        int y = grid.CellPerCol - 1;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            if (!grid.Cells[x, y].IsWalkable) continue;

            grid.Cells[x, y].BaseCost = COST_TARGET;
            flowFieldCellQueue.Enqueue(grid.Cells[x, y]);
        }
    }

    private void FindBottomEdgeTargetCells(CellGrid grid, Queue<Cell> flowFieldCellQueue)
    {
        int y = 0;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            if (!grid.Cells[x, y].IsWalkable) continue;

            grid.Cells[x, y].BaseCost = COST_TARGET;
            flowFieldCellQueue.Enqueue(grid.Cells[x, y]);
        }
    }

}
