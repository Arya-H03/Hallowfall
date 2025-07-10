
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "GraveYardLayoutProfile", menuName = "Scriptable Objects/GraveYardLayoutProfile")]

public class GraveYardLayoutProfile : ZoneLayoutProfile
{
    public RuleTile leafRuleTile;

    public TileBase[] graveStoneTiles;
    public TileBase[] graveDirtTiles;


    public Sprite[] skullSprites;
    public GameObject skullPrefab;
    public GameObject [] skullSpikesPrefabs;
    public GameObject [] cryptPrefabs;

    public GameObject flameHolderPrefab;
    
}
