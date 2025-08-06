using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum PropsBlockEnum
{
    none,
    graveCluster,
    treeCluster,
    cryptCluster,
    ritualCluster
}

[System.Serializable]
public struct PropsBlockStruct
{
    public PropsBlockEnum propsBlockEnum;
    public Vector2Int minBlockSize;
    public Vector2Int maxBlockSize;
}

[CreateAssetMenu(fileName = "ZoneLayoutProfile", menuName = "Scriptable Objects/ZoneLayoutProfile")]
public class ZoneLayoutProfile : ScriptableObject
{
    [Header("Block Types")]
    public List<PropsBlockStruct> propsBlocksStructList;
    public GameObject spawnablePropsBlock;

    [Header("Global Tiles")]
    public RuleTile fenceRuleTile;
    public RuleTile stoneRoadRuleTile;
    public TileBase grassRuletile;
    public TileBase defaultGroundTile;

    [Header("Grave Cluster")]
    public TileBase defaultDirtTile;
    public TileBase[] skullTiles;
    public TileBase[] graveStoneTiles;
    public TileBase[] graveDirtTiles;

    [Header("Tree Cluster")]
    public RuleTile leavesRuleTile;
    public TileBase[] treeTiles;
    [Range(0f, 1f)] public float treeDensity = 1f;

    [Header("Crypt Cluster")]
    public GameObject[] skullSpikesPrefabs;
    public GameObject[] cryptPrefabs;
    public GameObject flameHolderPrefab;

    [Header("Ritual Cluster")]
    public GameObject chalicePrefab;
    public GameObject candlePrefab;

    [Header("Earth ShakeCamera Effect")]
    public GameObject groundShakeEffectPrefab;
    public ParticleSystem groundShakeParticleEffectPrefab;

    [Header("Global Settings")]
    [Range(0f, 1f)] public float clutterDensity = 0.3f;
    public LayerMask propsMask;

    public GameObject GetRandomProps(GameObject[] props)
    {
        if (props.Length > 0)
            return props[Random.Range(0, props.Length)];
        return null;
    }

    public Sprite GetRandomSprite(Sprite[] sprites)
    {
        if (sprites.Length > 0)
            return sprites[Random.Range(0, sprites.Length)];
        return null;
    }

    public TileBase GetRandomTile(TileBase[] tiles, bool canReturnNothing)
    {
        if (tiles == null || tiles.Length == 0)
            return null;

        if (!canReturnNothing || Random.value > 0.25f)
            return tiles[Random.Range(0, tiles.Length)];

        return null;
    }
}
