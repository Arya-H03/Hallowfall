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
public class ZoneManager : MonoBehaviour
{
    private GameObject player;

    private int zoneSize = 40;
    private int halfZoneSize;
    private float zoneBuffer = 10f;

    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private GameObject mainGrid;


    [SerializeField] private GameObject initialZone;

    public Dictionary<Vector2Int,ZoneData> generatedZones = new Dictionary<Vector2Int, ZoneData>();


    private void Start()
    {
        player = GameManager.Instance.Player;
        halfZoneSize = zoneSize / 2;
        generatedZones.Add(FindCurrentPlayerZoneCoord(), new ZoneData(FindCurrentPlayerZoneCoord(), FindZoneCenterPosition(FindCurrentPlayerZoneCoord()), initialZone));
        initialZone.GetComponent<Zone>().ZoneData = generatedZones[FindCurrentPlayerZoneCoord()];    
        
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
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 0));
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1));
                break;
            case ZoneExpansionDir.Left:
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 0));
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1));
                break;
            case ZoneExpansionDir.Up:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, 1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, 1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, 1));
                break;
            case ZoneExpansionDir.Down:
                TryGenerateZone(currentZoneCoord + new Vector2Int(0, -1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(1, -1));
                TryGenerateZone(currentZoneCoord + new Vector2Int(-1, -1));
                break;
        }
    }

    private void TryGenerateZone(Vector2Int centerCoord)
    {
        if(!generatedZones.ContainsKey(centerCoord))
        {
            Vector3 newZonePos = FindZoneCenterPosition(centerCoord);
            GameObject zoneGO = Instantiate(zonePrefab, newZonePos, Quaternion.identity);
            ZoneData zoneData = new ZoneData(centerCoord, newZonePos, zoneGO);
            zoneGO.GetComponent<Zone>().ZoneData = zoneData;
            zoneGO.transform.parent = mainGrid.transform;
            generatedZones.Add(centerCoord, zoneData);
        }
    }

}
