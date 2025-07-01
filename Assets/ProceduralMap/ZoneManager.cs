
using NavMeshPlus.Components;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


[System.Serializable]
public class ZoneTypeProfiles
{
    public ZoneType zoneType;
    public ZoneLayoutProfile zoneLayoutProfile;
}

public class ZoneManager : MonoBehaviour
{
    private static ZoneManager instance;

    public static ZoneManager Instance
    {
        get 
        {
            return instance;
        }
    }
    private GameObject player;

    private int zoneSize = 40;
    private int halfZoneSize;
    private float zoneBuffer = 10f;

    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private GameObject mainGrid;

    [SerializeField] private ZoneConfig zoneConfig;

    [SerializeField] private Tilemap propsTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap grassTilemap;
    [SerializeField] private Tilemap stoneTilemap;
    [SerializeField] private Tilemap boundsTilemap;

    [SerializeField]
    public Dictionary<Vector2Int, ZoneData> generatedZones = new Dictionary<Vector2Int, ZoneData>();
     public NavMeshSurface navMeshSurface;

    [SerializeField] private List<ZoneTypeProfiles> zoneTypeProfiles;

    private Dictionary<ZoneType, ZoneLayoutProfile> zoneLayoutProfiles;

    public Dictionary<ZoneType, ZoneLayoutProfile> ZoneLayoutProfiles { get => zoneLayoutProfiles; }
    public Tilemap PropsTilemap { get => propsTilemap;}
    public Tilemap GroundTilemap { get => groundTilemap; }
    public Tilemap GrassTilemap { get => grassTilemap; }
    public Tilemap StoneTilemap { get => stoneTilemap; }
    public Tilemap BoundsTilemap { get => boundsTilemap; }

    private void Awake()
    {
        if (instance != null && instance !=this)
        {
            Destroy(this.gameObject);
        }
        instance = this;


        zoneLayoutProfiles = new Dictionary<ZoneType, ZoneLayoutProfile>();
        foreach (var profile in zoneTypeProfiles)
        {
            if (!ZoneLayoutProfiles.ContainsKey(profile.zoneType)) ZoneLayoutProfiles.Add(profile.zoneType, profile.zoneLayoutProfile);
        }
        halfZoneSize = zoneSize / 2;


    }

    private void Start()
    {
        player = GameManager.Instance.Player;

        TryGenerateZone((Vector2Int.zero), DirectionEnum.None);
        //navMeshSurface.BuildNavMesh();
    }

  

    private void Update()
    {
        CheckForPlayerEdgeProximity();
        if (Input.GetKeyDown(KeyCode.B))
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        }
    }

    private Vector2Int FindCurrentPlayerZoneCoord()
    {
        Vector3 pos = player.transform.position;
        Vector2Int currentZoneCoord = new Vector2Int(Mathf.FloorToInt((pos.x + halfZoneSize) / zoneSize), Mathf.FloorToInt((pos.y + halfZoneSize) / zoneSize));
     
        return currentZoneCoord;
    }

    private Vector2Int FindZoneCenterPosition(Vector2Int centerCoord)
    {
        return new Vector2Int(centerCoord.x * zoneSize, centerCoord.y * zoneSize);
    }
    private void CheckForPlayerEdgeProximity()
    {
        Vector2 currentZoneCenter = FindZoneCenterPosition(FindCurrentPlayerZoneCoord());

        Vector2 dist = (Vector2)player.transform.position - currentZoneCenter;
        if (Mathf.Abs(dist.x) > zoneBuffer && dist.x > 0) ExpandZones(DirectionEnum.Right);
        if (Mathf.Abs(dist.x) > zoneBuffer && dist.x < 0) ExpandZones(DirectionEnum.Left);
        if (Mathf.Abs(dist.y) > zoneBuffer && dist.y > 0) ExpandZones(DirectionEnum.Up);
        if (Mathf.Abs(dist.y) > zoneBuffer && dist.y < 0) ExpandZones(DirectionEnum.Down);

    }

    private void ExpandZones(DirectionEnum dir)
    {
        Vector2Int currentZoneCoord = FindCurrentPlayerZoneCoord();

        switch (dir)
        {
            case DirectionEnum.Right:
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 0), DirectionEnum.Left);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1), DirectionEnum.Left);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1), DirectionEnum.Left);
                break;
            case DirectionEnum.Left:
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 0), DirectionEnum.Right);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1), DirectionEnum.Right);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1), DirectionEnum.Right);
                break;
            case DirectionEnum.Up:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1), DirectionEnum.Down);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1), DirectionEnum.Down);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1), DirectionEnum.Down);
                break;
            case DirectionEnum.Down:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1), DirectionEnum.Up);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1), DirectionEnum.Up);
                //TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1), DirectionEnum.Up);
                break;
        }
    }

    private void TryGenerateZone(Vector2Int centerCoord, DirectionEnum expansionDir)
    {
        if (!generatedZones.ContainsKey(centerCoord))
        {
            Vector2Int newZonePos = FindZoneCenterPosition(centerCoord);         
            ZoneType zoneType = GenerateZoneType(centerCoord);

            GameObject zoneGO = CreateZone(centerCoord, expansionDir, zoneType, newZonePos);
            zoneGO.transform.parent = mainGrid.transform;
        }
    }

    private GameObject CreateZone(Vector2Int centerCoord, DirectionEnum dir, ZoneType zoneType, Vector2Int pos)
    {
        ZoneLayoutProfile layoutProfile = zoneLayoutProfiles[zoneType];
        GameObject zone = Instantiate(zonePrefab, (Vector3Int)pos, Quaternion.identity);
        generatedZones.Add(centerCoord, new ZoneData(centerCoord, FindZoneCenterPosition(centerCoord), zone, dir, zoneType, layoutProfile));

       
        switch (zoneType)
        {
            case ZoneType.graveYard:
                GraveyardHandler zoneHandler;
                zoneHandler = zone.AddComponent<GraveyardHandler>();
                zoneHandler.ZoneData = generatedZones[centerCoord];
                zoneHandler.ZoneConfig = zoneConfig;
                zoneHandler.ZoneLayoutProfile = layoutProfile;
                break;
            case ZoneType.plain:
                ZoneHandler zoneHandler1;
                zoneHandler1 = zone.AddComponent<ZoneHandler>();
                zoneHandler1.ZoneData = generatedZones[centerCoord];
                zoneHandler1.ZoneConfig = zoneConfig;
                zoneHandler1.ZoneLayoutProfile = layoutProfile;

                break;
        }

        return zone;
    }
    private ZoneType GenerateZoneType(Vector2Int centerCoord)
    {
        //ZoneType[] availableTypes = new ZoneType[]
        //{
        //    ZoneType.plain,
        //    ZoneType.graveYard,
        //    //ZoneType.forest
        //};

        //ZoneType randomZoneType = availableTypes[Random.Range(0, availableTypes.Length)];
        //if (randomZoneType == ZoneType.plain) return ZoneType.plain;
        //else
        //{
        //    bool temp = true;
        //    foreach (var dir in AllDirections)
        //    {
        //        Vector2Int neighborCoord = centerCoord + dir;

        //        if (generatedZones.TryGetValue(neighborCoord, out ZoneData neighborZone))
        //        {
        //            if (neighborZone.zoneType == randomZoneType)
        //            {
        //                temp = false;
        //            }
        //        }
        //    }
        //    if (temp == true) return randomZoneType;
        //    else return ZoneType.plain;

        return ZoneType.graveYard;
    }

    }

    



