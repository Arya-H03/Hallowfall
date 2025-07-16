using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraveyardHandler : ZoneHandler
{
    private Dictionary<DirectionEnum, Vector2Int[]> zoneOpenings = new Dictionary<DirectionEnum, Vector2Int[]>();


    protected override IEnumerator GenerateZoneCoroutine()
    {

        //GenerateBoundsForTilemap();
        //GenerateRoads();
        PopulateZoneWithPropBlocks(celLGrid, zoneLayoutProfile);
        //yield return null;
        AddDefaultGroundTileForZone(zoneLayoutProfile);
        yield return null;
        celLGrid.PaintAllCells();
        //StartCoroutine(celLGrid.PaintAllCellsCoroutine());
        
        // yield return null;
        //ZoneManager.Instance.navMeshSurface.BuildNavMesh();


    }
    private void GenerateBoundsForTilemap()
    {
        Tilemap boundsTilemap = /*ZoneManager.Instance.BoundsTilemap;*/ this.BoundsTilemap;

        TilePaint[] tilePaints = { new TilePaint { /*tilemap = ZoneManager.Instance.GroundOneTilemap*/ tilemap = this.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile }, new TilePaint { /*tilemap = ZoneManager.Instance.BoundsTilemap*/  tilemap = this.BoundsTilemap, tileBase = zoneLayoutProfile.fenceRuleTile } };

        //Bottom
        DrawStraightLineOfTiles(new Vector2Int(0,0), new Vector2Int(celLGrid.CellPerRow - 1, 0) , tilePaints);
        //Top
        DrawStraightLineOfTiles(new Vector2Int(0, celLGrid.CellPerCol - 1), new Vector2Int(celLGrid.CellPerRow - 1, celLGrid.CellPerCol - 1), tilePaints);
        //Left
        DrawStraightLineOfTiles(new Vector2Int(0, 0), new Vector2Int(0, celLGrid.CellPerCol - 1) , tilePaints);
        //Right
        DrawStraightLineOfTiles(new Vector2Int(celLGrid.CellPerRow - 1, 0), new Vector2Int(celLGrid.CellPerRow - 1, celLGrid.CellPerCol - 1), tilePaints);


        List<DirectionEnum> dirs = MyUtils.GetAllDirectionEnumList();
        int openingCount = Random.Range(2, 3);
        // Always one horizontal + one vertical + one random
        List<DirectionEnum> openingDir = new List<DirectionEnum>();
        DirectionEnum horizontal = MyUtils.GetRandomHorizontalDirectionEnum();
        DirectionEnum vertical = MyUtils.GetRandomVerticalDirectionEnum();
        openingDir.Add(horizontal);
        openingDir.Add(vertical);
        dirs.Remove(horizontal);
        dirs.Remove(vertical);
        openingDir.Add(dirs[Random.Range(0, dirs.Count)]);

        zoneOpenings = CreateOpeningsInZone(openingDir,celLGrid,boundsTilemap);

        
    }

    private Dictionary<DirectionEnum, Vector2Int[]> CreateOpeningsInZone(List<DirectionEnum> openingDir, CellGrid cellGrid ,Tilemap tilemap)
    {
        Dictionary<DirectionEnum, Vector2Int[]> zoneOpenings = new Dictionary<DirectionEnum, Vector2Int[]>();
        // Openings for each selected side
        foreach (DirectionEnum dir in openingDir)
        {
            int[] openingIndices = GenerateRandomOpeningCells();
            Vector2Int[] openings = new Vector2Int[openingIndices.Length];
            zoneOpenings.Add(dir, openings);

            for (int i = 0; i < openingIndices.Length; i++)
            {
                Vector2Int cellCoord = Vector2Int.zero;

                switch (dir)
                {
                    case DirectionEnum.Bottom:
                        cellCoord = new Vector2Int(openingIndices[i], 0);
                        break;
                    case DirectionEnum.Top:
                        cellCoord = new Vector2Int(openingIndices[i], cellGrid.CellPerCol - 1);
                        break;
                    case DirectionEnum.Left:
                        cellCoord = new Vector2Int(0, openingIndices[i]);
                        break;
                    case DirectionEnum.Right:
                        cellCoord = new Vector2Int(cellGrid.CellPerRow - 1, openingIndices[i]);
                        break;
                }

                Vector3Int pos = TurnCellCoordToTilePos(cellCoord.x, cellCoord.y);     
                zoneOpenings[dir][i] = cellCoord;
                cellGrid.Cells[cellCoord.x, cellCoord.y].IsOccupied = true;
                cellGrid.Cells[cellCoord.x, cellCoord.y].RemoveTilePaint();
            }
        }

        return zoneOpenings;
    }
    private void GenerateRoads()
    {
       

        var opening1 = zoneOpenings.ElementAt(0);
        var opening2 = zoneOpenings.ElementAt(1);

        ConnectAllCenterJunctionPoints(opening1.Value, opening2.Value);

        // Handle third direction if it exists
        if (zoneOpenings.Count == 3)
        {
            var opening3 = zoneOpenings.ElementAt(2);
            DirectionEnum thirdDir = opening3.Key;

            if (thirdDir == DirectionEnum.Top || thirdDir == DirectionEnum.Bottom)
            {
                if (zoneOpenings.ContainsKey(DirectionEnum.Left))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Left], opening3.Value);

                if (zoneOpenings.ContainsKey(DirectionEnum.Right))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Right], opening3.Value);
            }

            if (thirdDir == DirectionEnum.Left || thirdDir == DirectionEnum.Right)
            {
                if (zoneOpenings.ContainsKey(DirectionEnum.Top))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Top], opening3.Value);

                if (zoneOpenings.ContainsKey(DirectionEnum.Bottom))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Bottom], opening3.Value);
            }
        }

       
        
    }

    private void ConnectAllCenterJunctionPoints(Vector2Int[] from, Vector2Int[] to)
    {
        //Tilemap stoneTilemap = ZoneManager.Instance.GroundOneTilemap;
        Tilemap stoneTilemap = GroundOneTilemap;
        int size = from.Length;
        for (int i = 0; i < size; i++)
        {
            ConnectTwoJunctionPoints(stoneTilemap,from[i], to[i]);         // Matching index
            ConnectTwoJunctionPoints(stoneTilemap,from[i], to[2 - i]);     // Flipped index
        }
    }

    private void ConnectTwoJunctionPoints(Tilemap stoneTilemap, Vector2Int p1, Vector2Int p2)
    {
        Vector2Int junction = GetOpeningJunctionPoint(p1, p2);
        TilePaint[] tilePaint = {new TilePaint {/* tilemap = ZoneManager.Instance.GroundOneTilemap*/  tilemap = this.GroundOneTilemap,  tileBase = zoneLayoutProfile.stoneRoadRuleTile }};
        DrawStraightLineOfTiles(p1, junction, tilePaint);
        DrawStraightLineOfTiles(p2, junction ,tilePaint);
    }
    private Vector2Int GetOpeningJunctionPoint(Vector2Int p1, Vector2Int p2)
    {
        Vector2Int result = new Vector2Int(p1.x, p2.y);
        if (result.x == 0 || result.y == 0 || result.x >= 39 || result.y >= 39) return new Vector2Int(p2.x, p1.y);
        return result;
    }

    private int[] GenerateRandomOpeningCells()
    {
        int rand = Random.Range(5, 35);
        return new int[] { rand - 1, rand, rand + 1 };

    }
   

}
