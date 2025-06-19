using UnityEngine;

[CreateAssetMenu(fileName = "ZoneConfig", menuName = "Scriptable Objects/ZoneConfig")]
public class ZoneConfig : ScriptableObject
{
    [Header("FenceTilemaps")]
    public GameObject[] fenceNorthOpen;
    public GameObject[] fenceSouthOpen;
    public GameObject[] fenceWestOpen;
    public GameObject[] fenceEastOpen;

    [Header("ForestTilemaps")]
    public GameObject[] forestNorthOpen;
    public GameObject[] forestSouthOpen;
    public GameObject[] forestWestOpen;
    public GameObject[] forestEastOpen;
}
