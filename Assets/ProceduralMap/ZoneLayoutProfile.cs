using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ZoneLayoutProfile", menuName = "Scriptable Objects/ZoneLayoutProfile")]
public class ZoneLayoutProfile : ScriptableObject
{
    public GameObject[] spawnableProps; 
    public GameObject[] spawnableEnemies;
    public RuleTile boundsRuletile;
    public RuleTile roadRuletile;
    public RuleTile grassRuletile;
    public GameObject boundsTilemapGO;
    public GameObject roadTilemapGO;

    [Range(0f, 1f)] public float clutterDensity = 0.3f;

    public LayerMask propsMask;

    public GameObject GetRandomProps()
    {
        if (spawnableProps.Length > 0) return spawnableProps[Random.Range(0, spawnableProps.Length)];
        else return null;

    }
}
