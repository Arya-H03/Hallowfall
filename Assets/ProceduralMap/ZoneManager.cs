using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Manages the procedural generation and activation of game zones based on player proximity.
/// </summary>
public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [SerializeField] private int zoneSize = 80;
    [SerializeField] private int zoneCellSize = 1;
    [SerializeField] private float zoneBuffer = 30f;
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private GameObject mainGrid;
    [SerializeField] private Tilemap zoneConnectingGround;
    [SerializeField] private ZoneLayoutProfile zoneLayoutProfile;

    private FlowFieldGenerator flowFieldGenerator;
    public NavMeshSurface navMeshSurface;

  
    public Tilemap ZoneConnectingGround => zoneConnectingGround;

    public Dictionary<Vector2Int, ZoneData> generatedZonesDic = new();

    private GameObject player;
    private int halfZoneSize;

    private void OnValidate()
    {
        // Validate required serialized fields
        MyUtils.ValidateFields(this, zonePrefab, nameof(zonePrefab));
        MyUtils.ValidateFields(this, mainGrid, nameof(mainGrid));
        MyUtils.ValidateFields(this, zoneConnectingGround, nameof(zoneConnectingGround));
    }

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        halfZoneSize = zoneSize / 2;
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        flowFieldGenerator = new FlowFieldGenerator();
        // Generate the initial zone at origin
        TryGenerateZone(Vector2Int.zero, DirectionEnum.None);
        //ObjectPoolManager.Instance.GenerateEnemyPools();

    }

    private void Update()
    {
        //CheckForPlayerEdgeProximity();
    }

    public ZoneData FindCurrentZoneBasedOnWorldPos(Vector3 worldPos)
    {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
        foreach (KeyValuePair<Vector2Int, ZoneData> pair in generatedZonesDic)
        {
            ZoneData zoneData = pair.Value;
            float dist = (pos - pair.Key).sqrMagnitude;
            if (dist < zoneData.zoneWidth * zoneData.zoneWidth) return zoneData;
        }

        return null;
    }

    public void RequestFlowFieldGenerationOnAGrid(ZoneData zoneData,bool shouldVisualize = false)
    {
        ZoneHandler zoneHandler = zoneData.ZoneHandler;
        flowFieldGenerator.GenerateFlowField(zoneHandler.CellGrid, GameManager.Instance.Player.transform.position);

        if(shouldVisualize) GridSystemDebugger.Instance.VisualizeCellFlowDirection(zoneHandler.CellGrid, zoneData.centerCoord);
        //if(shouldVisualize) GridSystemDebugger.Instance.VisualizeCellFlowCosts(zoneHandler.CellGrid, zoneData.centerCoord);


    }

    private void TryGenerateZone(Vector2Int centerCoord, DirectionEnum expansionDir)
    {
        if (generatedZonesDic.ContainsKey(centerCoord)) return;

        Vector3Int newZoneWorldPos = FindZoneCenterPosition(centerCoord);
        GameObject newZoneGO = Instantiate(zonePrefab, newZoneWorldPos, Quaternion.identity, mainGrid.transform);

        var zoneData = new ZoneData(zoneCellSize, zoneSize, zoneSize,centerCoord, newZoneWorldPos, newZoneGO, expansionDir, zoneLayoutProfile);
        generatedZonesDic.Add(centerCoord, zoneData);

        var zoneHandler = newZoneGO.GetComponent<ZoneHandler>();
        zoneHandler.Init(zoneData);
    }

  
    private Vector2Int GetCurrentZoneCenterCoord()
    {
        Vector3 pos = player.transform.position;
        return new Vector2Int(
            Mathf.FloorToInt((pos.x + halfZoneSize) / zoneSize),
            Mathf.FloorToInt((pos.y + halfZoneSize) / zoneSize)
        );
    }

    public ZoneHandler GetCurrentZoneHandler()
    {
        return generatedZonesDic[GetCurrentZoneCenterCoord()].ZoneHandler;
    }
 
    private Vector3Int FindZoneCenterPosition(Vector2Int centerCoord)
    {
        return new Vector3Int(centerCoord.x * zoneSize, centerCoord.y * zoneSize, 0);
    }

    /// <summary>
    /// Checks the player's proximity to the edge of the current zone and expands nearby zones as needed.
    /// </summary>
    private void CheckForPlayerEdgeProximity()
    {
        Vector3 playerPos = player.transform.position;
        Vector3Int currentZoneCenter = FindZoneCenterPosition(GetCurrentZoneCenterCoord());

        Vector2Int[] allDirections = MyUtils.GetAllDirectionsVectorArray();
        Dictionary<Vector2Int, DirectionEnum> directionDic = MyUtils.GetDirectionDicWithVectorKey();

        foreach (Vector2Int dir in allDirections)
        {
            Vector2Int nextCoord = GetCurrentZoneCenterCoord() + dir;
            Vector3Int nextZonePos = currentZoneCenter + ((Vector3Int)dir * halfZoneSize);
            float distSqr = (nextZonePos - playerPos).sqrMagnitude;
            bool withinBuffer = distSqr < zoneBuffer * zoneBuffer;

            if (!generatedZonesDic.ContainsKey(nextCoord))
            {               
                if (withinBuffer && directionDic.TryGetValue(dir, out DirectionEnum dirEnum))
                {
                    ExpandZones(dirEnum);
                }
            }
            else
            {
                var zone = generatedZonesDic[nextCoord];

                if (!withinBuffer && zone.IsZoneFullyGenerated)
                {
                    zone.zoneGO.SetActive(false);
                }
                else
                {
                    zone.zoneGO.SetActive(true);
                }
            }
        }
    }


    /// <summary>
    /// Expands zones in the given direction, including diagonals.
    /// </summary>
    private void ExpandZones(DirectionEnum dirEnum)
    {
        Vector2Int currentCoord = GetCurrentZoneCenterCoord();

        switch (dirEnum)
        {
            case DirectionEnum.Right:
                TryGenerateZone(currentCoord + Vector2Int.right, DirectionEnum.Left);
                break;
            case DirectionEnum.Left:
                TryGenerateZone(currentCoord + Vector2Int.left, DirectionEnum.Right);
                break;
            case DirectionEnum.Up:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.Down);
                break;
            case DirectionEnum.Down:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.Up);
                break;
            case DirectionEnum.UpRight:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.DownLeft);
                TryGenerateZone(currentCoord + new Vector2Int(1, 1), DirectionEnum.DownLeft);
                break;
            case DirectionEnum.UpLeft:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.DownRight);
                TryGenerateZone(currentCoord + new Vector2Int(-1, 1), DirectionEnum.DownRight);
                break;
            case DirectionEnum.DownRight:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.UpLeft);
                TryGenerateZone(currentCoord + new Vector2Int(1, -1), DirectionEnum.UpLeft);
                break;
            case DirectionEnum.DownLeft:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.UpRight);
                TryGenerateZone(currentCoord + new Vector2Int(-1, -1), DirectionEnum.UpRight);
                break;
        }
    }
}
