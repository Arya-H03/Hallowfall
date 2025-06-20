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

    private Dictionary<ZoneDir, Vector2Int[]> zoneOpenings = new Dictionary<ZoneDir, Vector2Int[]>();

    public Tilemap BoundsTilemap { get => boundsTilemap; set => boundsTilemap = value; }

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

    protected override void CreateBoundsTilemap()
    {
        ZoneDir dir = zoneData.previousZoneDir;
        switch (dir)
        {
            case ZoneDir.None:
                break;
            case ZoneDir.Left:
                GenerateTilemap(zoneConfig.fenceWestOpen);
                break;
            case ZoneDir.Right:
                GenerateTilemap(zoneConfig.fenceEastOpen);
                break;
            case ZoneDir.Down:
                GenerateTilemap(zoneConfig.fenceSouthOpen);
                break;
            case ZoneDir.Up:
                GenerateTilemap(zoneConfig.fenceNorthOpen);
                break;
        }
    }

    private void GenerateBoundsTilemap()
    {
        List<ZoneDir> dirs = new List<ZoneDir> { ZoneDir.Left, ZoneDir.Right, ZoneDir.Up, ZoneDir.Down };
        int openingCount = Random.Range(2, 3);

        // Always one horizontal + one vertical + one random
        List<ZoneDir> openingDir = new List<ZoneDir>();

        ZoneDir horizontal = new List<ZoneDir> { ZoneDir.Left, ZoneDir.Right }[Random.Range(0, 2)];
        ZoneDir vertical = new List<ZoneDir> { ZoneDir.Up, ZoneDir.Down }[Random.Range(0, 2)];

        openingDir.Add(horizontal);
        openingDir.Add(vertical);

        dirs.Remove(horizontal);
        dirs.Remove(vertical);

        openingDir.Add(dirs[Random.Range(0, dirs.Count)]);

        // Create border tiles
        for (int i = 0; i < cellsY; i++)
        {
            boundsTilemap.SetTile(PositionFromGridCell(i, 0), zoneLayoutProfile.boundsRuletile); // Down
            boundsTilemap.SetTile(PositionFromGridCell(i, cellsY - 1), zoneLayoutProfile.boundsRuletile); // Up
            boundsTilemap.SetTile(PositionFromGridCell(0, i), zoneLayoutProfile.boundsRuletile); // Left
            boundsTilemap.SetTile(PositionFromGridCell(cellsX - 1, i), zoneLayoutProfile.boundsRuletile); // Right

            cells[i, 0].isOccupied = false;
            cells[i, cellsY - 1].isOccupied = false;
            cells[0, i].isOccupied = false;
            cells[cellsX - 1, i].isOccupied = false;
        }

        // Openings for each selected side
        foreach (ZoneDir dir in openingDir)
        {
            int[] openingIndices = GenerateRandomOpeningCells();
            Vector2Int[] openings = new Vector2Int[openingIndices.Length];
            zoneOpenings.Add(dir, openings);

            for (int i = 0; i < openingIndices.Length; i++)
            {
                Vector2Int cellCoord = Vector2Int.zero;

                switch (dir)
                {
                    case ZoneDir.Down:
                        cellCoord = new Vector2Int(openingIndices[i], 0);
                        break;
                    case ZoneDir.Up:
                        cellCoord = new Vector2Int(openingIndices[i], cellsY - 1);
                        break;
                    case ZoneDir.Left:
                        cellCoord = new Vector2Int(0, openingIndices[i]);
                        break;
                    case ZoneDir.Right:
                        cellCoord = new Vector2Int(cellsX - 1, openingIndices[i]);
                        break;
                }

                boundsTilemap.SetTile(PositionFromGridCell(cellCoord.x, cellCoord.y), null);
                zoneOpenings[dir][i] = cellCoord;
                cells[cellCoord.x, cellCoord.y].isOccupied = true;
            }
        }

        GenerateRoadTileMap();
    }


    private void GenerateRoadTileMap()
    {
        SetupRoadTilemap();

        var firstKvp = zoneOpenings.ElementAt(0);
        var secondKvp = zoneOpenings.ElementAt(1);

        ConnectAll(firstKvp.Value, secondKvp.Value);

        // Handle third direction if it exists
        if (zoneOpenings.Count == 3)
        {
            var thirdKvp = zoneOpenings.ElementAt(2);
            ZoneDir thirdDir = thirdKvp.Key;

            if (thirdDir == ZoneDir.Up || thirdDir == ZoneDir.Down)
            {
                if (zoneOpenings.ContainsKey(ZoneDir.Left))
                    ConnectAll(zoneOpenings[ZoneDir.Left], thirdKvp.Value);

                if (zoneOpenings.ContainsKey(ZoneDir.Right))
                    ConnectAll(zoneOpenings[ZoneDir.Right], thirdKvp.Value);
            }

            if (thirdDir == ZoneDir.Left || thirdDir == ZoneDir.Right)
            {
                if (zoneOpenings.ContainsKey(ZoneDir.Up))
                    ConnectAll(zoneOpenings[ZoneDir.Up], thirdKvp.Value);

                if (zoneOpenings.ContainsKey(ZoneDir.Down))
                    ConnectAll(zoneOpenings[ZoneDir.Down], thirdKvp.Value);
            }
        }

        PaintUnoccupiedGround();
        PopulateZone();
    }

    private void SetupRoadTilemap()
    {
        GameObject roadTilemapGO = Instantiate(zoneLayoutProfile.roadTilemapGO, transform.position, Quaternion.identity);
        roadTilemapGO.transform.parent = transform;
        roadTilemap = roadTilemapGO.GetComponent<Tilemap>();
    }

    private void ConnectAll(Vector2Int[] from, Vector2Int[] to)
    {
        for (int i = 0; i < 3; i++)
        {
            Connect(from[i], to[i]);         // Matching index
            Connect(from[i], to[2 - i]);     // Flipped index
        }
    }

    private void Connect(Vector2Int p1, Vector2Int p2)
    {
        Vector2Int junction = GetOpeningJunctionPoint(p1, p2);

        DrawStraightRoad(p1, junction);
        DrawStraightRoad(p2, junction);

        roadTilemap.SetTile(PositionFromGridCell(junction.x, junction.y), zoneLayoutProfile.roadRuletile);
    }

    private void PaintUnoccupiedGround()
    {
        for (int y = 0; y < cellsY; y++)
        {
            for (int x = 0; x < cellsX; x++)
            {
                if (!cells[x, y].isOccupied)
                {
                    groundTilemap.SetTile(PositionFromGridCell(x, y), zoneLayoutProfile.grassRuletile);
                }
            }
        }
    }




    private void DrawStraightRoad(Vector2Int from, Vector2Int to)
    {
        Vector2Int delta = to - from;

        if (delta.x == 0) // Vertical road
        {
            int yMin = Mathf.Min(from.y, to.y);
            int yMax = Mathf.Max(from.y, to.y);

            for (int y = yMin; y < yMax +1; y++)
            {
                roadTilemap.SetTile(PositionFromGridCell(from.x, y), zoneLayoutProfile.roadRuletile);
                cells[from.x, y].isOccupied = true;
            }
        }
        else if (delta.y == 0) // Horizontal road
        {
            int xMin = Mathf.Min(from.x, to.x);
            int xMax = Mathf.Max(from.x, to.x);

            for (int x = xMin; x < xMax +1; x++)
            {
                roadTilemap.SetTile(PositionFromGridCell(x, from.y), zoneLayoutProfile.roadRuletile);
                cells[x, from.y].isOccupied = true;
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
    private Vector3Int PositionFromGridCell(int x, int y)
    {
        return new Vector3Int(
            Mathf.FloorToInt(x - 20),
            Mathf.FloorToInt(y - 20),
            0
        );
    }

}
