using System.Collections.Generic;
using UnityEngine;

public enum ZoneExpansionDir
{
    None,
    Right,
    Left,
    Up,
    Down
}

[System.Serializable]
public class ZoneTypeProfiles
{
    public ZoneType zoneType;
    public ZoneLayoutProfile zoneLayoutProfile;
}

public class ZoneManager : MonoBehaviour
{
    public static readonly Vector2Int[] AllDirections = new Vector2Int[]
{
    Vector2Int.right,
    Vector2Int.left,
    Vector2Int.up,
    Vector2Int.down,
    new Vector2Int(1, 1),
    new Vector2Int(1, -1),
    new Vector2Int(-1, 1),
    new Vector2Int(-1, -1),
};

    private GameObject player;

    private int zoneSize = 40;
    private int halfZoneSize;
    private float zoneBuffer = 10f;

    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private GameObject mainGrid;

    [SerializeField] private ZoneConfig zoneConfig;


    [SerializeField] private GameObject initialZone;

    public Dictionary<Vector2Int, ZoneData> generatedZones = new Dictionary<Vector2Int, ZoneData>();


    [SerializeField] private List<ZoneTypeProfiles> zoneTypeProfiles;

    private Dictionary<ZoneType, ZoneLayoutProfile> zoneLayoutProfiles;

    public Dictionary<ZoneType, ZoneLayoutProfile> ZoneLayoutProfiles { get => zoneLayoutProfiles; }

    private void Awake()
    {
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
        generatedZones.Add(FindCurrentPlayerZoneCoord(), new ZoneData(FindCurrentPlayerZoneCoord(), FindZoneCenterPosition(FindCurrentPlayerZoneCoord()), initialZone, ZoneExpansionDir.None, ZoneType.plain, zoneLayoutProfiles[ZoneType.plain]));
        initialZone.GetComponent<MapZone>().ZoneData = generatedZones[FindCurrentPlayerZoneCoord()];
        initialZone.GetComponent<MapZone>().ZoneConfig = zoneConfig;

    }

    private void Update()
    {
        CheckForPlayerEdgeProximity();
    }

    private Vector2Int FindCurrentPlayerZoneCoord()
    {
        Vector3 pos = player.transform.position;
        Vector2Int currentZoneCoord = new Vector2Int(Mathf.FloorToInt((pos.x + halfZoneSize) / zoneSize), Mathf.FloorToInt((pos.y + halfZoneSize) / zoneSize));
        return currentZoneCoord;
    }

    private Vector2 FindZoneCenterPosition(Vector2Int centerCoord)
    {
        return new Vector2(centerCoord.x * zoneSize, centerCoord.y * zoneSize);
    }
    private void CheckForPlayerEdgeProximity()
    {
        Vector2 currentZoneCenter = FindZoneCenterPosition(FindCurrentPlayerZoneCoord());

        Vector2 dist = (Vector2)player.transform.position - currentZoneCenter;
        if (Mathf.Abs(dist.x) > zoneBuffer && dist.x > 0) ExpandZones(ZoneExpansionDir.Right);
        if (Mathf.Abs(dist.x) > zoneBuffer && dist.x < 0) ExpandZones(ZoneExpansionDir.Left);
        if (Mathf.Abs(dist.y) > zoneBuffer && dist.y > 0) ExpandZones(ZoneExpansionDir.Up);
        if (Mathf.Abs(dist.y) > zoneBuffer && dist.y < 0) ExpandZones(ZoneExpansionDir.Down);

    }

    private void ExpandZones(ZoneExpansionDir dir)
    {
        Vector2Int currentZoneCoord = FindCurrentPlayerZoneCoord();

        switch (dir)
        {
            case ZoneExpansionDir.Right:
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 0), ZoneExpansionDir.Left);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1), ZoneExpansionDir.Left);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1), ZoneExpansionDir.Left);
                break;
            case ZoneExpansionDir.Left:
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 0), ZoneExpansionDir.Right);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1), ZoneExpansionDir.Right);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1), ZoneExpansionDir.Right);
                break;
            case ZoneExpansionDir.Up:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1), ZoneExpansionDir.Down);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1), ZoneExpansionDir.Down);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1), ZoneExpansionDir.Down);
                break;
            case ZoneExpansionDir.Down:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1), ZoneExpansionDir.Up);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1), ZoneExpansionDir.Up);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1), ZoneExpansionDir.Up);
                break;
        }
    }

    private void TryGenerateZone(Vector2Int centerCoord, ZoneExpansionDir expansionDir)
    {
        if (!generatedZones.ContainsKey(centerCoord))
        {
            Vector3 newZonePos = FindZoneCenterPosition(centerCoord);
            GameObject zoneGO = Instantiate(zonePrefab, newZonePos, Quaternion.identity);
            ZoneType zoneType = GenerateZoneType(centerCoord);
            ZoneData zoneData = new ZoneData(centerCoord, newZonePos, zoneGO, expansionDir, zoneType, zoneLayoutProfiles[zoneType]);
            MapZone mapZone = zoneGO.GetComponent<MapZone>();
            mapZone.ZoneData = zoneData;
            mapZone.ZoneConfig = zoneConfig;
            zoneGO.transform.parent = mainGrid.transform;
            generatedZones.Add(centerCoord, zoneData);
            mapZone.CreateZoneEnvironemnt();
        }
    }

    private ZoneType GenerateZoneType(Vector2Int centerCoord)
    {
        ZoneType[] availableTypes = new ZoneType[]
        {
            ZoneType.plain,
            ZoneType.graveYard,
            ZoneType.forest
        };

        ZoneType randomZoneType = availableTypes[Random.Range(0, availableTypes.Length)];
        if (randomZoneType == ZoneType.plain) return ZoneType.plain;
        else
        {
            bool temp = true;
            foreach (var dir in AllDirections)
            {
                Vector2Int neighborCoord = centerCoord + dir;

                if (generatedZones.TryGetValue(neighborCoord, out ZoneData neighborZone))
                {
                    if (neighborZone.zoneType == randomZoneType)
                    {
                        temp = false;
                    }
                }
            }
            if (temp == true) return randomZoneType;
            else return ZoneType.plain;
        }



    }


}
