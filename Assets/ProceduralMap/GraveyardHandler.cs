using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GraveyardHandler : ZoneHandler
{
    private Tilemap boundsTilemap;
    private Tilemap roadTilemap;

    private Dictionary<ZoneDir, Vector2Int[]> zoneOpenings = new Dictionary<ZoneDir, Vector2Int[]>();

    public Tilemap BoundsTilemap { get => boundsTilemap; set => boundsTilemap = value; }

    protected override void Awake()
    {
        base.Awake();
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
        int openingCount = Random.Range(2, 2);

        List<ZoneDir> openingDir = new List<ZoneDir>();
        List<ZoneDir> hDir = new List<ZoneDir>() { ZoneDir.Left, ZoneDir.Right };
        openingDir.Add(hDir[Random.Range(0, hDir.Count)]);
        List<ZoneDir> VDir = new List<ZoneDir>() { ZoneDir.Down, ZoneDir.Up };
        openingDir.Add(VDir[Random.Range(0, VDir.Count)]);

        //for (int i = 0; i < openingCount; i++)
        //{
        //    int randIndex = Random.Range(0, dirs.Count);
        //    openingDir.Add(dirs[randIndex]);
        //    dirs.RemoveAt(randIndex);
        //}

        for (int i = 0; i < cellsY - 1; i++)
        {
            // Bottom (Down)
            boundsTilemap.SetTile(PositionFromGridCell(i, 0), zoneLayoutProfile.boundsRuletile);
            cells[i, 0].isOccupied = true;
            // Top (Up)
            boundsTilemap.SetTile(PositionFromGridCell(i, cellsY - 2), zoneLayoutProfile.boundsRuletile);
            cells[i, cellsY - 2].isOccupied = true;
            // Left
            boundsTilemap.SetTile(PositionFromGridCell(0, i), zoneLayoutProfile.boundsRuletile);
            cells[0, i].isOccupied = true;
            // Right
            boundsTilemap.SetTile(PositionFromGridCell(cellsX - 1, i), zoneLayoutProfile.boundsRuletile);
            cells[cellsX - 1, i].isOccupied = true;
        }

        // Openings
        if (openingDir.Contains(ZoneDir.Down))
        {
            int[] openingCells = GenerateRandomOpeningCells();
            zoneOpenings.Add(ZoneDir.Down, new Vector2Int[openingCells.Length]);

            for (int i = 0; i < openingCells.Length; i++)
            {
                boundsTilemap.SetTile(PositionFromGridCell(openingCells[i], 0), null);
                zoneOpenings[ZoneDir.Down][i] = new Vector2Int(openingCells[i], 0);
                cells[openingCells[i], 0].isOccupied = false;
            }
        }

        if (openingDir.Contains(ZoneDir.Up))
        {
            int[] openingCells = GenerateRandomOpeningCells();
            zoneOpenings.Add(ZoneDir.Up, new Vector2Int[openingCells.Length]);

            for (int i = 0; i < openingCells.Length; i++)
            {
                boundsTilemap.SetTile(PositionFromGridCell(openingCells[i], cellsY - 2), null);
                zoneOpenings[ZoneDir.Up][i] = new Vector2Int(openingCells[i], cellsY - 2);
                cells[openingCells[i], cellsY - 2].isOccupied = false;
            }
        }

        if (openingDir.Contains(ZoneDir.Left))
        {
            int[] openingCells = GenerateRandomOpeningCells();
            zoneOpenings.Add(ZoneDir.Left, new Vector2Int[openingCells.Length]);

            for (int i = 0; i < openingCells.Length; i++)
            {
                boundsTilemap.SetTile(PositionFromGridCell(0, openingCells[i]), null);
                zoneOpenings[ZoneDir.Left][i] = new Vector2Int(0, openingCells[i]);
                cells[0, openingCells[i]].isOccupied = false;
            }
        }

        if (openingDir.Contains(ZoneDir.Right))
        {
            int[] openingCells = GenerateRandomOpeningCells();
            zoneOpenings.Add(ZoneDir.Right, new Vector2Int[openingCells.Length]);

            for (int i = 0; i < openingCells.Length; i++)
            {
                boundsTilemap.SetTile(PositionFromGridCell(cellsX - 1, openingCells[i]), null);
                zoneOpenings[ZoneDir.Right][i] = new Vector2Int(cellsX - 1, openingCells[i]);
                cells[cellsX - 1, openingCells[i]].isOccupied = false;
            }
        }


        GenerateRoadTileMap();
    }

    private void GenerateRoadTileMap()
    {
        GameObject roadTilemapGO = Instantiate(zoneLayoutProfile.roadTilemapGO, transform.position, Quaternion.identity);
        roadTilemapGO.transform.parent = transform;
        roadTilemap = roadTilemapGO.GetComponent<Tilemap>();

        var firstKvp = zoneOpenings.ElementAt(0);
        var secondKvp = zoneOpenings.ElementAt(1);

        

        for (int i = 0; i < 3; i++)
        {
            Vector2Int junction = GetOpeningJunctionPoint(firstKvp.Value[i], secondKvp.Value[i]);
            DrawStraightRoad(firstKvp.Value[i], junction);
            DrawStraightRoad(secondKvp.Value[i] , junction);

            roadTilemap.SetTile(PositionFromGridCell(junction.x, junction.y), zoneLayoutProfile.roadRuletile);
        }

  
        PopulateZone();
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

            for (int x = xMin; x < xMax; x++)
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
