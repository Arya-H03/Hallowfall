using UnityEngine;

[System.Serializable]
public class ZoneData
{
    public Vector2Int centerCoord;
    public Vector2 centerPos;
    public GameObject zoneGO;

    public ZoneData (Vector2Int centerCoord, Vector2 centerPos, GameObject zoneGO)
    {
        this.centerCoord = centerCoord;
        this.centerPos = centerPos;
        this.zoneGO = zoneGO;
    }
    public ZoneData(Vector2Int centerCoord, Vector2 centerPos)
    {
        this.centerCoord = centerCoord;
        this.centerPos = centerPos;
    }
}
