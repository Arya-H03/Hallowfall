using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public Vector2Int centerCoord;
    public Vector3Int centerPos;
    public GameObject zoneGO;
    public DirectionEnum previousZoneDir;
    public ZoneLayoutProfile zoneProfile;

    public ZoneData (Vector2Int centerCoord, Vector3Int centerPos, GameObject zoneGO, DirectionEnum previousZoneDir,ZoneLayoutProfile zoneProfile)
    {
        this.centerCoord = centerCoord;
        this.centerPos = centerPos;
        this.zoneGO = zoneGO;
        this.previousZoneDir = previousZoneDir;
        this.zoneProfile = zoneProfile;
    }
    
}
