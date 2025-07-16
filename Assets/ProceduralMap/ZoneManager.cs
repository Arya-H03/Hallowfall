
using NavMeshPlus.Components;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [SerializeField] private int zoneSize = 80;
    [SerializeField] private int zoneCellSize = 1;
    [SerializeField] private float zoneBuffer = 30f;

    private int halfZoneSize;

    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private GameObject mainGrid;

    public NavMeshSurface navMeshSurface;


    //[SerializeField] private Tilemap groundZeroTilemap;
    //[SerializeField] private Tilemap groundOneTilemap;
    //[SerializeField] private Tilemap groundTwoTilemap;

    //[SerializeField] private Tilemap boundsTilemap;
    //[SerializeField] private Tilemap propsTilemap;
    //[SerializeField] private Tilemap treeTilemap;

    [SerializeField]
    public Dictionary<Vector2Int, ZoneData> generatedZonesDic = new Dictionary<Vector2Int, ZoneData>();

    [SerializeField] private List<ZoneTypeProfiles> zoneTypeProfiles;

    private Dictionary<ZoneType, ZoneLayoutProfile> zoneLayoutProfiles;

    public Dictionary<ZoneType, ZoneLayoutProfile> ZoneLayoutProfiles { get => zoneLayoutProfiles; }
    //public Tilemap PropsTilemap { get => propsTilemap; }
    //public Tilemap GroundZeroTilemap { get => groundZeroTilemap; }

    //public Tilemap BoundsTilemap { get => boundsTilemap; }
    //public Tilemap TreeTilemap { get => treeTilemap; private set => treeTilemap = value; }
    //public Tilemap GroundOneTilemap { get => groundOneTilemap; private set => groundOneTilemap = value; }
    //public Tilemap GroundTwoTilemap { get => groundTwoTilemap; private set => groundTwoTilemap = value; }

    private void OnValidate()
    {
        MyUtils.ValidateFields(this, zonePrefab, nameof(zonePrefab));
        MyUtils.ValidateFields(this, mainGrid, nameof(mainGrid));


        //MyUtils.ValidateFields(this, propsTilemap, nameof(propsTilemap));
        //MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));
        //MyUtils.ValidateFields(this, groundOneTilemap, nameof(groundZeroTilemap));
        //MyUtils.ValidateFields(this, groundZeroTilemap, nameof(groundZeroTilemap));

        //MyUtils.ValidateFields(this, boundsTilemap, nameof(boundsTilemap));
        //MyUtils.ValidateFields(this, TreeTilemap, nameof(TreeTilemap));
    }

    private void Awake()
    {
        if (instance != null && instance != this)
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

    }



    private void Update()
    {
        CheckForPlayerEdgeProximity();

    }

    private void TryGenerateZone(Vector2Int centerCoord, DirectionEnum expansionDir)
    {
        if (!generatedZonesDic.ContainsKey(centerCoord))
        {
            Vector2Int newZoneWorldPos = FindZoneCenterPosition(centerCoord);
            ZoneType zoneType = GenerateZoneType(centerCoord);

            GameObject zoneGO = CreateZone(centerCoord, expansionDir, zoneType, newZoneWorldPos);
            zoneGO.transform.parent = mainGrid.transform;
        }
    }

    private Vector2Int FindCurrentZoneCenterCoord()
    {
        Vector3 pos = player.transform.position;
        Vector2Int currentZoneCoord = new Vector2Int(Mathf.FloorToInt((pos.x + halfZoneSize) / zoneSize), Mathf.FloorToInt((pos.y + halfZoneSize) / zoneSize));

        return currentZoneCoord;
    }

    private Vector2Int FindZoneCenterPosition(Vector2Int centerCoord)
    {
        return new Vector2Int(centerCoord.x * zoneSize, centerCoord.y * zoneSize);
    }

    private ZoneType GenerateZoneType(Vector2Int centerCoord)
    {
        //    ZoneType[] availableTypes = new ZoneType[]
        //    {
        //        ZoneType.openField,
        //        ZoneType.graveYard,
        //    };

        //    ZoneType randomZoneType = availableTypes[Random.Range(0, availableTypes.Length)];
        //    if (randomZoneType == ZoneType.openField) return ZoneType.openField;
        //    else
        //    {
        //        bool temp = true;
        //        foreach (var dir in MyUtils.GetCardinalDirectionsVector())
        //        {
        //            Vector2Int neighborCoord = centerCoord + dir;

        //            if (generatedZonesDic.TryGetValue(neighborCoord, out ZoneData neighborZone))
        //            {
        //                if (neighborZone.zoneType == randomZoneType)
        //                {
        //                    temp = false;
        //                }
        //            }
        //        }
        //        if (temp == true) return randomZoneType;
        //        else return ZoneType.openField;


        //    }

        return ZoneType.graveYard;
    }

    private GameObject CreateZone(Vector2Int centerCoord, DirectionEnum dir, ZoneType zoneType, Vector2Int pos)
    {
        ZoneLayoutProfile layoutProfile = zoneLayoutProfiles[zoneType];
        GameObject zone = Instantiate(zonePrefab, (Vector3Int)pos, Quaternion.identity);
        generatedZonesDic.Add(centerCoord, new ZoneData(centerCoord, FindZoneCenterPosition(centerCoord), zone, dir, zoneType, layoutProfile));


        switch (zoneType)
        {
            case ZoneType.graveYard:
                GraveyardHandler garveYardHandler = zone.GetComponent<GraveyardHandler>();
                //garveYardHandler = zone.AddComponent<GraveyardHandler>();
                garveYardHandler.Init(generatedZonesDic[centerCoord], layoutProfile, zoneSize, zoneSize, zoneCellSize);

                break;
            case ZoneType.openField:
                OpenFieldHandler openFieldHandler;
                openFieldHandler = zone.AddComponent<OpenFieldHandler>();
                openFieldHandler.Init(generatedZonesDic[centerCoord], layoutProfile, zoneSize, zoneSize, zoneCellSize);

                break;
        }

        return zone;
    }

    private void CheckForPlayerEdgeProximity()
    {
        Vector3 playerPos = player.transform.position;   
        Vector2 currentZoneCenter = FindZoneCenterPosition(FindCurrentZoneCenterCoord());
       
        Vector2Int[] allDirection = MyUtils.GetAllDirectionsVector();
        Dictionary<Vector2Int, DirectionEnum> directionDic = MyUtils.GetDirectionDicWithVectorKey();
        
        foreach(Vector2Int dir  in allDirection)
        {
            Vector2Int nextZoneCenterCoord = FindCurrentZoneCenterCoord() + dir;
            Vector3 nextZoneCenterPos = currentZoneCenter + (dir * halfZoneSize);
            float dist = (nextZoneCenterPos - playerPos).sqrMagnitude;
            if (!generatedZonesDic.ContainsKey(nextZoneCenterCoord))
            {              
                if (dist < zoneBuffer * zoneBuffer)
                {
                    DirectionEnum dirEnum = directionDic[dir];
                    ExpandZones(dirEnum);
                }
            }
            else
            {
                bool shouldZoneBeActive = dist < zoneBuffer * zoneBuffer;
                generatedZonesDic[nextZoneCenterCoord].zoneGO.SetActive(shouldZoneBeActive);
              
            }
            
        }
      
    }

    private void ExpandZones(DirectionEnum dirEnum)
    {
        Vector2Int currentZoneCoord = FindCurrentZoneCenterCoord();

        switch (dirEnum)
        {
            case DirectionEnum.Right:
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 0), DirectionEnum.Left);               
                break;
            case DirectionEnum.Left:
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 0), DirectionEnum.Right);               
                break;
            case DirectionEnum.Top:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1), DirectionEnum.Bottom);             
                break;
            case DirectionEnum.Bottom:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1), DirectionEnum.Top);             
                break;
            case DirectionEnum.TopRight:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1), DirectionEnum.BottomLeft);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1), DirectionEnum.BottomLeft);     
                break;
            case DirectionEnum.TopLeft:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1), DirectionEnum.BottomRight);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1), DirectionEnum.BottomRight);
                break;
            case DirectionEnum.BottomRight:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1), DirectionEnum.TopLeft);
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1), DirectionEnum.TopLeft);
                break;
            case DirectionEnum.BottomLeft:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1), DirectionEnum.TopRight);
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1), DirectionEnum.TopRight);
                break;
        }
    }




}






