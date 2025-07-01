using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum PropsBlockEnum
{
    graveCluster,
    treeCluster,
    cryptCluster
}
[System.Serializable]
public struct PropsBlockStruct
{
    public PropsBlockEnum propsBlockEnum;
    public MonoScript scriptReference;
    public Vector2Int minBlockSize;
}

[CreateAssetMenu(fileName = "ZoneLayoutProfile", menuName = "Scriptable Objects/ZoneLayoutProfile")]
public class ZoneLayoutProfile : ScriptableObject
{
    public List<PropsBlockStruct> propsBlocksStructList;

    public GameObject   spawnablePropsBlock;
  
    public RuleTile boundsRuletile;
    public RuleTile roadRuletile;
    public RuleTile grassRuletile;
    public RuleTile treeRuletile;
    public RuleTile groundRuletile;


    public TileBase[] treeTiles;

    [Range(0f, 1f)] public float clutterDensity = 0.3f;

    public LayerMask propsMask;

    public GameObject GetRandomProps(GameObject[] props)
    {
        if (props.Length > 0) return props[Random.Range(0, props.Length)];
        else return null;

    }

    public Sprite GetRandomSprite(Sprite[] sprites)
    {
        if (sprites.Length > 0) return sprites[Random.Range(0, sprites.Length)];
        else return null;

    }

    public TileBase GetRandomTile(TileBase[] tiles, bool canReturnNothing)
    {
        if (tiles == null || tiles.Length == 0)
            return null;

        if (!canReturnNothing || Random.value > 0.25f)
        {
            return tiles[Random.Range(0, tiles.Length)];
        }

        return null;


    }
}
