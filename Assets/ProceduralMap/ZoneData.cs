using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public int cellSize;
    public int zoneWidth;
    public int zoneHeight;
    public Vector2Int centerCoord;
    public Vector3Int centerPos;
    public GameObject zoneGO;
    public DirectionEnum previousZoneDir;
    public ZoneLayoutProfile zoneLayoutProfile;
    private bool isZoneFullyGenerated = false;

    public ZoneData(int cellSize, int zoneWidth, int zoneHeight, Vector2Int centerCoord, Vector3Int centerPos, GameObject zoneGO, DirectionEnum previousZoneDir,ZoneLayoutProfile zoneProfile)
    {
        this.cellSize = cellSize;
        this.zoneWidth = zoneWidth;
        this.zoneHeight = zoneHeight;
        this.centerCoord = centerCoord;
        this.centerPos = centerPos;
        this.zoneGO = zoneGO;
        this.previousZoneDir = previousZoneDir;
        this.zoneLayoutProfile = zoneProfile;
    }

    public bool IsZoneFullyGenerated { get => isZoneFullyGenerated; set => isZoneFullyGenerated = value; }
}
