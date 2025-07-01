
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
    public GameObject cryptPrefab;
    


    //public GameObject GenerateRandomGraveStone()
    //{
    //    //GameObject go = Instantiate(gravestoneBasePrefab);
    //    //if(graveStoneSprites.Length > 0 && graveDirtSprites.Length > 0)
    //    //{
    //    //    go.GetComponent<SpriteRenderer>().sprite = graveStoneSprites[Random.Range(0, graveStoneSprites.Length)];
            
    //    //    if(Random.Range(1,7) > 4)
    //    //    {
    //    //        go.transform.GetChild(0).gameObject.SetActive(true);
    //    //        go.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = graveDirtSprites[Random.Range(0, graveDirtSprites.Length)];
    //    //    }
            
    //    //}
        
    //    //return go;
    //}
}
