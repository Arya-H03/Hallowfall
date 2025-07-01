using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraveyardHandler : ZoneHandler
{
    private Dictionary<DirectionEnum, Vector2Int[]> zoneOpenings = new Dictionary<DirectionEnum, Vector2Int[]>();


    protected override void Awake()
    {
        base.Awake();
       
    }

    protected override void Start()
    {
        base.Start();
       
       
    }

    protected override IEnumerator GenerateZoneCoroutine()
    {
        GenerateBoundsForTilemap();
        GenerateRoads();
        //PaintAllCells();
        PopulateZoneWithPropBlocks(celLGrid, zoneLayoutProfile);



        //yield return null;
        //PaintUnoccupiedGround(ZoneManager.Instance.GroundTilemap, zoneLayoutProfile.grassRuletile, celLGrid);
        //yield return null;

        celLGrid.PaintAllCells();
        yield return null;
    }
    private void GenerateBoundsForTilemap()
    {
        Tilemap boundsTilemap = ZoneManager.Instance.BoundsTilemap;

        TilePaint[] tilePaints = { new TilePaint { tilemap = ZoneManager.Instance.GroundTilemap, tileBase = zoneLayoutProfile.grassRuletile }, new TilePaint { tilemap = ZoneManager.Instance.BoundsTilemap, tileBase = zoneLayoutProfile.boundsRuletile } };

        //Down
        DrawStraightLineOfTiles(new Vector2Int(0,0), new Vector2Int(celLGrid.CellPerRow - 1, 0) , tilePaints);
        //Up
        DrawStraightLineOfTiles(new Vector2Int(0, celLGrid.CellPerCol - 1), new Vector2Int(celLGrid.CellPerRow - 1, celLGrid.CellPerCol - 1), tilePaints);
        //Left
        DrawStraightLineOfTiles(new Vector2Int(0, 0), new Vector2Int(0, celLGrid.CellPerCol - 1) , tilePaints);
        //Right
        DrawStraightLineOfTiles(new Vector2Int(celLGrid.CellPerRow - 1, 0), new Vector2Int(celLGrid.CellPerRow - 1, celLGrid.CellPerCol - 1), tilePaints);


        List<DirectionEnum> dirs = ProceduralUtils.GetAllDirectionList();
        int openingCount = Random.Range(2, 3);
        // Always one horizontal + one vertical + one random
        List<DirectionEnum> openingDir = new List<DirectionEnum>();
        DirectionEnum horizontal = ProceduralUtils.GetRandomHorizontalDirectionEnum();
        DirectionEnum vertical = ProceduralUtils.GetRandomVerticalDirectionEnum();
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
                    case DirectionEnum.Down:
                        cellCoord = new Vector2Int(openingIndices[i], 0);
                        break;
                    case DirectionEnum.Up:
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
                ////tilemap.SetTile(pos, null);
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

            if (thirdDir == DirectionEnum.Up || thirdDir == DirectionEnum.Down)
            {
                if (zoneOpenings.ContainsKey(DirectionEnum.Left))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Left], opening3.Value);

                if (zoneOpenings.ContainsKey(DirectionEnum.Right))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Right], opening3.Value);
            }

            if (thirdDir == DirectionEnum.Left || thirdDir == DirectionEnum.Right)
            {
                if (zoneOpenings.ContainsKey(DirectionEnum.Up))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Up], opening3.Value);

                if (zoneOpenings.ContainsKey(DirectionEnum.Down))
                    ConnectAllCenterJunctionPoints(zoneOpenings[DirectionEnum.Down], opening3.Value);
            }
        }

       
        
    }

    private void ConnectAllCenterJunctionPoints(Vector2Int[] from, Vector2Int[] to)
    {
        Tilemap stoneTilemap = ZoneManager.Instance.StoneTilemap;
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
        TilePaint[] tilePaint = {new TilePaint { tilemap = ZoneManager.Instance.StoneTilemap, tileBase = zoneLayoutProfile.roadRuletile }};
        DrawStraightLineOfTiles(p1, junction, tilePaint);
        DrawStraightLineOfTiles(p2, junction ,tilePaint);

        //Vector3Int pos = TurnCellCoordToTilePos(junction.x, junction.y);
        ////stoneTilemap.SetTile(pos, zoneLayoutProfile.roadRuletile);
    }

    private void PaintUnoccupiedGround(Tilemap tilemap, RuleTile ruleTile, CellGrid cellGrid )
    {
        for (int y = 0; y < cellGrid.CellPerCol; y++)
        {
            for (int x = 0; x < cellGrid.CellPerRow; x++)
            {
                if (!cellGrid.Cells[x, y].IsOccupied)
                {
                    Vector3Int pos = TurnCellCoordToTilePos(x, y);
                    tilemap.SetTile(pos, ruleTile);
                }
            }
        }
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
