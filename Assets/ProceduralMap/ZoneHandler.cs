
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class ZoneHandler : MonoBehaviour
{
 
    private ZoneData zoneData;
    private ZoneLayoutProfile zoneLayoutProfile;
    private int cellSize;
    private int zoneWidth;
    private int zoneHeight;
  
    private CellGrid cellGrid;
    private readonly List<BoundsInt> listOfSubzoneBounds = new();
    private List<BoundsInt> listOfPartitionedSubzoneBounds = new();
    private Dictionary<DirectionEnum, Vector2Int[]> zoneOpenings = new();

 
    [SerializeField] private Tilemap groundZeroTilemap;
    [SerializeField] private Tilemap groundOneTilemap;
    [SerializeField] private Tilemap groundTwoTilemap;
    [SerializeField] private Tilemap boundsTilemap;
    [SerializeField] private Tilemap propsWithCollisionTilemap;
    [SerializeField] private Tilemap propsNoCollisionTilemap;
    [SerializeField] private Tilemap treeTilemap;

    public Tilemap GroundZeroTilemap => groundZeroTilemap;
    public Tilemap GroundOneTilemap => groundOneTilemap;
    public Tilemap GroundTwoTilemap => groundTwoTilemap;
    public Tilemap PropsWithCollisionTilemap => propsWithCollisionTilemap;
    public Tilemap PropsNoCollisionTilemap => propsNoCollisionTilemap;
    public Tilemap BoundsTilemap => boundsTilemap;
    public Tilemap TreeTilemap => treeTilemap;
   

    public ZoneData ZoneData { get => zoneData; set => zoneData = value; }
    public ZoneLayoutProfile ZoneLayoutProfile { get => zoneLayoutProfile; set => zoneLayoutProfile = value; }
   

    private void OnValidate()
    {

        MyUtils.ValidateFields(this, propsWithCollisionTilemap, nameof(propsWithCollisionTilemap));
        MyUtils.ValidateFields(this, propsNoCollisionTilemap, nameof(propsNoCollisionTilemap));
        MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));
        MyUtils.ValidateFields(this, groundOneTilemap, nameof(groundZeroTilemap));
        MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));

        MyUtils.ValidateFields(this, boundsTilemap, nameof(boundsTilemap));
        MyUtils.ValidateFields(this, TreeTilemap, nameof(TreeTilemap));
    }

    public void Init(ZoneData zoneData)
    {
        this.zoneData = zoneData;
        this.zoneLayoutProfile = this.zoneData.zoneLayoutProfile;
        this.zoneWidth = this.zoneData.zoneWidth;
        this.zoneHeight = this.zoneData.zoneHeight;
        this.cellSize = this.zoneData.cellSize;

        cellGrid = new CellGrid(this.cellSize, this.zoneWidth, this.zoneHeight);
        cellGrid.InitializeGridCells(this.zoneData.centerPos);

        //groundOneTilemap = ZoneManager.Instance.ZoneConnectingGround;

        StartCoroutine(GenerateZoneCoroutine());

    }
    private IEnumerator GenerateZoneCoroutine()
    {

        //GenerateBoundsForTilemap();
        //GenerateRoads();

        PopulateZoneWithPropBlocks(cellGrid, zoneLayoutProfile);
        AddDefaultGroundTileForZone(zoneLayoutProfile);
        yield return null;

        //cellGrid.PaintAllCells();
        
        yield return StartCoroutine(cellGrid.PaintGrid());
        //yield return StartCoroutine(cellGrid.PaintAllCellsCoroutine());


        //ZoneManager.Instance.navMeshSurface.BuildNavMesh();
      
        zoneData.IsZoneFullyGenerated = true;
    }
   


    private void PopulateZoneWithPropBlocks(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        Cell startCell = cellGrid.FindNextUnpartitionedCell(new Vector2Int(0, 0));
        CreateAllSubZoneBounds(cellGrid, startCell);

        //Turns big subZones to smalle blocks 
        listOfPartitionedSubzoneBounds = MyUtils.PerformeBinarySpacePartitioning(listOfSubzoneBounds, 8, 8);

        InstantiatePropsBlocks(listOfPartitionedSubzoneBounds, zoneLayoutProfile);
    }


    
    private void CreateAllSubZoneBounds(CellGrid cellGrid, Cell startCell)
    {
        //Check along left edge
        int h1 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x, startCell.GlobalCellCoord.y + i].IsPartitioned)
                break;
            h1++;
        }
        //Check along bottom edge
        int w1 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + i, startCell.GlobalCellCoord.y].IsPartitioned)
                break;
            w1++;
        }
        //Check along top edge
        int w2 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.x < cellGrid.CellPerRow; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + i, startCell.GlobalCellCoord.y + h1 - 1].IsPartitioned)
                break;
            w2++;
        }

        int width = Mathf.Min(w1, w2);
        //Check along right edge
        int h2 = 1;
        for (int i = 1; i + startCell.GlobalCellCoord.y < cellGrid.CellPerCol; i++)
        {
            if (cellGrid.Cells[startCell.GlobalCellCoord.x + width - 1, startCell.GlobalCellCoord.y + i].IsPartitioned)
                break;
            h2++;
        }

        int height = Mathf.Min(h1, h2);

        listOfSubzoneBounds.Add(new BoundsInt(new Vector3Int((int)startCell.GlobalCellPos.x, (int)startCell.GlobalCellPos.y, 0), new Vector3Int(width, height, 0)));

        
        for (int x = startCell.GlobalCellCoord.x; x < startCell.GlobalCellCoord.x + width; x++)
        {
            for (int y = startCell.GlobalCellCoord.y; y < startCell.GlobalCellCoord.y + height; y++)
            {
                if (x >= 0 && x < cellGrid.CellPerRow && y >= 0 && y < cellGrid.CellPerCol)
                    cellGrid.Cells[x, y].IsPartitioned = true;
            }
        }

        Cell nextStartCell = cellGrid.FindNextUnpartitionedCell(new Vector2Int(startCell.GlobalCellCoord.x + width, startCell.GlobalCellCoord.y));
        if (nextStartCell != null)
        {
            CreateAllSubZoneBounds(cellGrid, nextStartCell);
        }
    }

    private void InstantiatePropsBlocks(List<BoundsInt> listOfPartitionedSubzoneBounds, ZoneLayoutProfile zoneLayoutProfile)
    {
        foreach (BoundsInt zoneBounds in listOfPartitionedSubzoneBounds)
        {
            PropsBlockEnum propsBlockEnum = GetPropsBlockTypeEnum(zoneLayoutProfile,zoneBounds);
            if(propsBlockEnum != PropsBlockEnum.none)
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zoneBounds.position, Quaternion.identity);
                go.transform.parent = this.transform;
                PropsBlock propsBlock = AddBlockComponent(go, propsBlockEnum);
                propsBlock.Init(this, cellGrid, zoneBounds.position,  zoneBounds, zoneLayoutProfile);

            }
            else
            {
                GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, zoneBounds.position, Quaternion.identity);
                go.transform.parent = this.transform;
            }
                



        }
    }

    private PropsBlockEnum GetPropsBlockTypeEnum(ZoneLayoutProfile zoneLayoutProfile, BoundsInt blockBounds)
    {
        if (zoneLayoutProfile.propsBlocksStructList.Count < 1) return PropsBlockEnum.none;


        PropsBlockEnum propsBlockEnum = PropsBlockEnum.none;
        int maxAttempts = 100;
        int attempts = 0;

        while (propsBlockEnum == PropsBlockEnum.none && attempts < maxAttempts)
        {
            PropsBlockStruct propsBlock = zoneLayoutProfile.propsBlocksStructList[
                Random.Range(1, zoneLayoutProfile.propsBlocksStructList.Count)];

            if (blockBounds.size.x >= propsBlock.minBlockSize.x &&
                blockBounds.size.y >= propsBlock.minBlockSize.y &&
                propsBlock.propsBlockEnum != PropsBlockEnum.none)
            {
                propsBlockEnum = propsBlock.propsBlockEnum;
            }

            attempts++;
        }

        return propsBlockEnum;

      
    }

    private PropsBlock AddBlockComponent(GameObject propsBlockGO, PropsBlockEnum propsBlockEnum)
    {
        switch (propsBlockEnum)
        {
            case PropsBlockEnum.cryptCluster:
                return propsBlockGO.AddComponent<CryptClusterBlock>();

            case PropsBlockEnum.graveCluster:
                return propsBlockGO.AddComponent<GraveClusterBlock>();

            case PropsBlockEnum.treeCluster:
                return propsBlockGO.AddComponent<TreeClusterBlock>();

            case PropsBlockEnum.ritualCluster:
                return propsBlockGO.AddComponent<RitualClusterBlock>();

            default:
                return null;
        }
    }


    private void AddDefaultGroundTileForZone(ZoneLayoutProfile zoneLayoutProfile)
    {

        CellPaint tilePaint = new CellPaint {tilemap = this.GroundZeroTilemap, tileBase = zoneLayoutProfile.defaultGroundTile };
        cellGrid.LoopOverGrid((i, j) =>
        {
            cellGrid.Cells[i, j].AddToCellPaint(tilePaint);
        });
    }

    private void DrawStraightLineOfTiles(Vector2Int beginningCellCoord, Vector2Int endCellCoord, CellPaint[] tilePaints)
    {
        Vector2Int delta = endCellCoord - beginningCellCoord;

        if (delta.x == 0) // Vertical road
        {

            int yMin = Mathf.Min(beginningCellCoord.y, endCellCoord.y);
            int yMax = Mathf.Max(beginningCellCoord.y, endCellCoord.y);

            for (int y = yMin; y < yMax + 1; y++)
            {
                Vector3Int pos = TurnCellCoordToTilePos(beginningCellCoord.x, y);
                //tilemap.SetTile(pos, tileBase);
                cellGrid.Cells[beginningCellCoord.x, y].IsOccupied = true;
                cellGrid.Cells[beginningCellCoord.x, y].IsPartitioned = true;
                cellGrid.Cells[beginningCellCoord.x, y].AddToCellPaint(tilePaints);
            }
        }
        else if (delta.y == 0) // Horizontal road
        {
            int xMin = Mathf.Min(beginningCellCoord.x, endCellCoord.x);
            int xMax = Mathf.Max(beginningCellCoord.x, endCellCoord.x);

            for (int x = xMin; x < xMax + 1; x++)
            {

                Vector3Int pos = TurnCellCoordToTilePos(x, beginningCellCoord.y);
                //tilemap.SetTile(pos, tileBase);
                cellGrid.Cells[x, beginningCellCoord.y].IsOccupied = true;
                cellGrid.Cells[x, beginningCellCoord.y].IsPartitioned = true;
                cellGrid.Cells[x, beginningCellCoord.y].AddToCellPaint(tilePaints);
            }
        }
    }

    private Vector3Int TurnCellCoordToTilePos(int x, int y)
    {
        return (Vector3Int)cellGrid.Cells[x, y].GlobalCellPos;
    }


    private void GenerateBoundsForTilemap()
    {
        Tilemap boundsTilemap = /*ZoneManager.Instance.BoundsTilemap;*/ this.BoundsTilemap;

        CellPaint[] tilePaints = { new CellPaint { /*tilemap = ZoneManager.Instance.GroundOneTilemap*/ tilemap = this.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile }, new CellPaint { /*tilemap = ZoneManager.Instance.BoundsTilemap*/  tilemap = this.BoundsTilemap, tileBase = zoneLayoutProfile.fenceRuleTile } };

        //Bottom
        DrawStraightLineOfTiles(new Vector2Int(0, 0), new Vector2Int(cellGrid.CellPerRow - 1, 0), tilePaints);
        //Top
        DrawStraightLineOfTiles(new Vector2Int(0, cellGrid.CellPerCol - 1), new Vector2Int(cellGrid.CellPerRow - 1, cellGrid.CellPerCol - 1), tilePaints);
        //Left
        DrawStraightLineOfTiles(new Vector2Int(0, 0), new Vector2Int(0, cellGrid.CellPerCol - 1), tilePaints);
        //Right
        DrawStraightLineOfTiles(new Vector2Int(cellGrid.CellPerRow - 1, 0), new Vector2Int(cellGrid.CellPerRow - 1, cellGrid.CellPerCol - 1), tilePaints);


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

        zoneOpenings = CreateOpeningsInZone(openingDir, cellGrid, boundsTilemap);


    }

    private Dictionary<DirectionEnum, Vector2Int[]> CreateOpeningsInZone(List<DirectionEnum> openingDir, CellGrid cellGrid, Tilemap tilemap)
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
                cellGrid.Cells[cellCoord.x, cellCoord.y].RemoveCellPaints();
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
            ConnectTwoJunctionPoints(stoneTilemap, from[i], to[i]);         // Matching index
            ConnectTwoJunctionPoints(stoneTilemap, from[i], to[2 - i]);     // Flipped index
        }
    }

    private void ConnectTwoJunctionPoints(Tilemap stoneTilemap, Vector2Int p1, Vector2Int p2)
    {
        Vector2Int junction = GetOpeningJunctionPoint(p1, p2);
        CellPaint[] tilePaint = { new CellPaint {/* tilemap = ZoneManager.Instance.GroundOneTilemap*/  tilemap = this.GroundOneTilemap, tileBase = zoneLayoutProfile.stoneRoadRuleTile } };
        DrawStraightLineOfTiles(p1, junction, tilePaint);
        DrawStraightLineOfTiles(p2, junction, tilePaint);
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

    private void VisualizeGridCells(SubCellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        cellGrid.LoopOverGrid((i, j) =>
        {
            GameObject go = Instantiate(zoneLayoutProfile.spawnablePropsBlock, (Vector3Int)cellGrid.Cells[j, i].GlobalCellPos, Quaternion.identity);

            if (!cellGrid.Cells[j, i].IsOccupied)
            {
                go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
            }
            else if (cellGrid.Cells[j, i].IsOccupied)
            {
                go.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
            }
        });
    }
}


