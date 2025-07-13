
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "GraveYardLayoutProfile", menuName = "Scriptable Objects/GraveYardLayoutProfile")]

public class GraveYardLayoutProfile : ZoneLayoutProfile
{
   
    [Header("Grave Cluster")]
    public Sprite[] skullSprites;
    public GameObject skullPrefab;
    public TileBase defaultDirtTile;
    public TileBase[] graveStoneTiles;
    public TileBase[] graveDirtTiles;

    [Header("Tree Cluster")]
    public RuleTile leavesRuleTile;
    public TileBase[] treeTiles;
    public float treeDensity = 1f;

    [Header("Crypt Cluster")]
    public GameObject[] skullSpikesPrefabs;
    public GameObject[] cryptPrefabs;
    public GameObject flameHolderPrefab;

    [Header("Earth Shake Up")]
    public GameObject groundShakeEffectPrefab;
    public ParticleSystem groundShakeParticleEffectPrefab;

}
