using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraveyardHandler : ZoneHandler
{
    private Tilemap boundsTilemap;
    private Tilemap roadTilemap;
    private Tilemap groundTilemap;

    private Dictionary<DirectionEnum, Vector2Int[]> zoneOpenings = new Dictionary<DirectionEnum, Vector2Int[]>();


    protected override void Awake()
    {
        base.Awake();
        groundTilemap = transform.GetChild(0).GetComponentInChildren<Tilemap>();
    }

    protected override void Start()
    {
        base.Start();
        GenerateBoundsTilemap();
    }
    private void GenerateBoundsTilemap()
    {
        boundsTilemap = CreateTilemap(zoneLayoutProfile.boundsTilemapGO, this.transform);

        //Down
        DrawStraightLineOfTiles(Vector2Int.zero, new Vector2Int(cellsX-1, 0),zoneLayoutProfile.boundsRuletile,boundsTilemap);
        //Up
        DrawStraightLineOfTiles(new Vector2Int(0, cellsY - 1), new Vector2Int(cellsX-1, cellsY - 1),zoneLayoutProfile.boundsRuletile,boundsTilemap);
        //Left
        DrawStraightLineOfTiles(Vector2Int.zero, new Vector2Int(0, cellsY-1),zoneLayoutProfile.boundsRuletile,boundsTilemap);
        //Right
        DrawStraightLineOfTiles(new Vector2Int(cellsX -1, 0), new Vector2Int(cellsX - 1, cellsY-1),zoneLayoutProfile.boundsRuletile, boundsTilemap);


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

        zoneOpenings = CreateOpeningsInZone(openingDir,zoneCells,boundsTilemap);

        GenerateRoadTileMap();
    }

    private Dictionary<DirectionEnum, Vector2Int[]> CreateOpeningsInZone(List<DirectionEnum> openingDir, Cell[,] cells ,Tilemap tilemap)
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
                        cellCoord = new Vector2Int(openingIndices[i], cellsY - 1);
                        break;
                    case DirectionEnum.Left:
                        cellCoord = new Vector2Int(0, openingIndices[i]);
                        break;
                    case DirectionEnum.Right:
                        cellCoord = new Vector2Int(cellsX - 1, openingIndices[i]);
                        break;
                }

                tilemap.SetTile(PositionFromGridCell(cellCoord.x, cellCoord.y), null);
                zoneOpenings[dir][i] = cellCoord;
                cells[cellCoord.x, cellCoord.y].isOccupied = true;
            }
        }

        return zoneOpenings;
    }
    private void GenerateRoadTileMap()
    {
        roadTilemap = CreateTilemap(zoneLayoutProfile.roadTilemapGO, this.transform);

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

        PaintUnoccupiedGround(groundTilemap,zoneLayoutProfile.grassRuletile,zoneCells);
        PopulateZoneWithPropBlocks(zoneCells,zoneLayoutProfile);
    }

    private void ConnectAllCenterJunctionPoints(Vector2Int[] from, Vector2Int[] to)
    {
        int size = from.Length;
        for (int i = 0; i < size; i++)
        {
            ConnectTwoJunctionPoints(from[i], to[i]);         // Matching index
            ConnectTwoJunctionPoints(from[i], to[2 - i]);     // Flipped index
        }
    }

    private void ConnectTwoJunctionPoints(Vector2Int p1, Vector2Int p2)
    {
        Vector2Int junction = GetOpeningJunctionPoint(p1, p2);

        DrawStraightLineOfTiles(p1, junction, zoneLayoutProfile.roadRuletile,roadTilemap);
        DrawStraightLineOfTiles(p2, junction, zoneLayoutProfile.roadRuletile, roadTilemap);

        roadTilemap.SetTile(PositionFromGridCell(junction.x, junction.y), zoneLayoutProfile.roadRuletile);
    }

    private void PaintUnoccupiedGround(Tilemap tilemap, RuleTile ruleTile, Cell[,] cells )
    {
        for (int y = 0; y < cells.GetLength(1); y++)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                if (!cells[x, y].isOccupied)
                {
                    tilemap.SetTile(PositionFromGridCell(x, y), ruleTile);
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
