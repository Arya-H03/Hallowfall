using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlowFieldGenerator
{
    public void GenerateFlowFieldOnTargetZone(CellGrid cellGrid, Vector3 targetPos)
    {
        Queue<Cell> flowFieldCellQueue = new Queue<Cell>();

        Cell targetCell = cellGrid.GetCellFromWorldPos(targetPos);

        SetInitialBaseFlowCosts(cellGrid, targetCell, flowFieldCellQueue);

        BFS(cellGrid, flowFieldCellQueue);

        AssignFlowAllDirectionsOnPlayerZone(cellGrid);

    }

    public void GenerateFlowFieldOnNonePlayerZone(CellGrid cellGrid, DirectionEnum dirToPlayerZone)
    {
        Queue<Cell> flowFieldCellQueue = new Queue<Cell>();
        
        FindAllEdgeCells(cellGrid, flowFieldCellQueue, dirToPlayerZone);

        BFS(cellGrid, flowFieldCellQueue);

        AssignFlowAllDirectionsOnNonePlayerZone(cellGrid, dirToPlayerZone);
    }


    private void FindAllEdgeCells(CellGrid cellGrid, Queue<Cell> flowFieldCellQueue, DirectionEnum dirToPlayerZone)
    {
        List<Cell> edgeCells = new List<Cell>();

        cellGrid.LoopOverGrid((i, j) =>
        {
            Cell currentCell = cellGrid.Cells[i, j];
            if (!currentCell.IsWalkable)
            {
                currentCell.BaseCost = (int)CellFlowCost.unWalkable;
            }
            else currentCell.BaseCost = (int)CellFlowCost.unVisited;
        });


        switch (dirToPlayerZone)
        {
            case DirectionEnum.Right:
                AssignRightEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.Left:
                AssignLeftEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.Up:
                AssignTopEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.Down:
                AssignBottomEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.UpRight:
                AssignTopEdgesToList(cellGrid, edgeCells);
                AssignRightEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.UpLeft:
                AssignTopEdgesToList(cellGrid, edgeCells);
                AssignLeftEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.DownRight:
                AssignBottomEdgesToList(cellGrid, edgeCells);
                AssignRightEdgesToList(cellGrid, edgeCells);
                break;
            case DirectionEnum.DownLeft:
                AssignBottomEdgesToList(cellGrid, edgeCells);
                AssignLeftEdgesToList(cellGrid, edgeCells);
                break;
        }

        foreach(Cell edgeCell in edgeCells)
        {
            if (!edgeCell.IsWalkable) continue;
            edgeCell.BaseCost = (int)CellFlowCost.target;
            flowFieldCellQueue.Enqueue(edgeCell);
        }
    }


    private void SetInitialBaseFlowCosts(CellGrid cellGrid, Cell targetCell, Queue<Cell> flowFieldCellQueue)
    {
        cellGrid.LoopOverGrid((i, j) =>
        {
            Cell currentCell = cellGrid.Cells[i, j];

            if (!currentCell.IsWalkable)
            {
                currentCell.BaseCost = (int)CellFlowCost.unWalkable;
            }
            else
            {
                currentCell.BaseCost = (int)CellFlowCost.unVisited;

            }
        }
        );

        targetCell.BaseCost = (int)CellFlowCost.target;
        flowFieldCellQueue.Enqueue(targetCell);
    }

    private void BFS(CellGrid cellGrid, Queue<Cell> flowFieldCellQueue)
    {
        while (flowFieldCellQueue.Count > 0)
        {
            Cell currentCell = flowFieldCellQueue.Dequeue();
            foreach (Cell neighbor in GetAllValidNeighbors(cellGrid, currentCell))
            {
                if (neighbor.BaseCost == (int)CellFlowCost.unVisited)
                {
                    neighbor.BaseCost = currentCell.BaseCost + 1;
                    flowFieldCellQueue.Enqueue(neighbor);

                }
            }
        }
    }

    private List<Cell> GetAllValidNeighbors(CellGrid cellGrid, Cell centerCell)
    {
        List<Cell> result = new List<Cell>();
        foreach (Cell neighborCell in centerCell.GetAllNeighborCells())
        {
            if (/*neighborCell.IsWalkable &&*/ neighborCell.BaseCost == (int)CellFlowCost.unVisited)
            {
                result.Add(neighborCell);
            }
        }
        return result;


    }

    private void AssignFlowAllDirectionsOnPlayerZone(CellGrid cellGrid)
    {
       
        cellGrid.LoopOverGrid((i, j) =>
        {
            cellGrid.Cells[i, j].FlowDir = FindFlowForPlayerDirection(cellGrid, cellGrid.Cells[i, j]);
            
        }
        );
    }

    private void AssignFlowAllDirectionsOnNonePlayerZone(CellGrid cellGrid, DirectionEnum directionEnum)
    {

        cellGrid.LoopOverGrid((i, j) =>
        {
            cellGrid.Cells[i, j].FlowDir = FindFlowDirectionForNonePlayerZone(cellGrid, cellGrid.Cells[i, j], directionEnum);
           
        }
        );
    }

    private DirectionEnum FindFlowForPlayerDirection(CellGrid cellGrid, Cell currentCell)
    {
        if (currentCell.TotalCost == (int) CellFlowCost.target) return DirectionEnum.None;

        Cell cheapestNeighbor = null;

        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (neighbor.TotalCost < 0 || !neighbor.IsWalkable)
            {
               
                continue;
            } 


            if (cheapestNeighbor == null || neighbor.TotalCost < cheapestNeighbor.TotalCost)
            {
                
                cheapestNeighbor = neighbor;
            }
        }

        if (cheapestNeighbor == null)
        {
           
            return DirectionEnum.None;
        }

        Vector2Int dirToCheapestNeighbor = (cheapestNeighbor.GlobalCellCoord - currentCell.GlobalCellCoord);

        return MyUtils.GetDirFromVector(dirToCheapestNeighbor);
    }

    private DirectionEnum FindFlowDirectionForNonePlayerZone(CellGrid cellGrid, Cell currentCell,DirectionEnum directionEnum)
    {
        if (currentCell.TotalCost == (int)CellFlowCost.target) return directionEnum;

        Cell cheapestNeighbor = null;

        foreach (Cell neighbor in currentCell.GetAllNeighborCells())
        {
            if (neighbor.TotalCost < 0 || !neighbor.IsWalkable)
            {

                continue;
            }


            if (cheapestNeighbor == null || neighbor.TotalCost < cheapestNeighbor.TotalCost)
            {

                cheapestNeighbor = neighbor;
            }
        }

        if (cheapestNeighbor == null)
        {

            return DirectionEnum.None;
        }

        Vector2Int dirToCheapestNeighbor = (cheapestNeighbor.GlobalCellCoord - currentCell.GlobalCellCoord);

        return MyUtils.GetDirFromVector(dirToCheapestNeighbor);
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




    private void AssignRightEdgesToList(CellGrid grid, List<Cell> list)
    {
        int x = grid.CellPerRow - 1;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            list.Add(grid.Cells[x, y]);
        }
    }

    private void AssignLeftEdgesToList(CellGrid grid, List<Cell> list)
    {
        int x = 0;
        for (int y = 0; y < grid.CellPerCol; y++)
        {
            list.Add(grid.Cells[x, y]);
        }
    }

    private void AssignTopEdgesToList(CellGrid grid, List<Cell> list)
    {
        int y = grid.CellPerCol - 1;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            list.Add(grid.Cells[x, y]);
        }
    }

    private void AssignBottomEdgesToList(CellGrid grid, List<Cell> list)
    {
        int y = 0;
        for (int x = 0; x < grid.CellPerRow; x++)
        {
            list.Add(grid.Cells[x, y]);
        }
    }

    //private void TryEnqueue(Cell cell,Queue<Cell> Queue)
    //{
    //    if (!cell.IsWalkable) return;

    //    CellFlowCost
    //}
}
