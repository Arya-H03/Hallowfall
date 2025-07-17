using NavMeshPlus.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        // Generate the initial zone at origin
        TryGenerateZone(Vector2Int.zero, DirectionEnum.None);
    }

    private void Update()
    {
        CheckForPlayerEdgeProximity();
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

  
    private Vector2Int FindCurrentZoneCenterCoord()
    {
        Vector3 pos = player.transform.position;
        return new Vector2Int(
            Mathf.FloorToInt((pos.x + halfZoneSize) / zoneSize),
            Mathf.FloorToInt((pos.y + halfZoneSize) / zoneSize)
        );
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
        Vector3Int currentZoneCenter = FindZoneCenterPosition(FindCurrentZoneCenterCoord());

        Vector2Int[] allDirections = MyUtils.GetAllDirectionsVector();
        Dictionary<Vector2Int, DirectionEnum> directionDic = MyUtils.GetDirectionDicWithVectorKey();

        foreach (Vector2Int dir in allDirections)
        {
            Vector2Int nextCoord = FindCurrentZoneCenterCoord() + dir;
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
        Vector2Int currentCoord = FindCurrentZoneCenterCoord();

        switch (dirEnum)
        {
            case DirectionEnum.Right:
                TryGenerateZone(currentCoord + Vector2Int.right, DirectionEnum.Left);
                break;
            case DirectionEnum.Left:
                TryGenerateZone(currentCoord + Vector2Int.left, DirectionEnum.Right);
                break;
            case DirectionEnum.Top:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.Bottom);
                break;
            case DirectionEnum.Bottom:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.Top);
                break;
            case DirectionEnum.TopRight:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.BottomLeft);
                TryGenerateZone(currentCoord + new Vector2Int(1, 1), DirectionEnum.BottomLeft);
                break;
            case DirectionEnum.TopLeft:
                TryGenerateZone(currentCoord + Vector2Int.up, DirectionEnum.BottomRight);
                TryGenerateZone(currentCoord + new Vector2Int(-1, 1), DirectionEnum.BottomRight);
                break;
            case DirectionEnum.BottomRight:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.TopLeft);
                TryGenerateZone(currentCoord + new Vector2Int(1, -1), DirectionEnum.TopLeft);
                break;
            case DirectionEnum.BottomLeft:
                TryGenerateZone(currentCoord + Vector2Int.down, DirectionEnum.TopRight);
                TryGenerateZone(currentCoord + new Vector2Int(-1, -1), DirectionEnum.TopRight);
                break;
        }
    }
}
