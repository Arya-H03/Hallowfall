using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

[CreateAssetMenu(fileName = "ZoneLayoutProfile", menuName = "Scriptable Objects/ZoneLayoutProfile")]
public class ZoneLayoutProfile : ScriptableObject
{
    public List<Type> propsBlockClassList = new List<Type>()
    {
        { typeof(GraveClusterBlock) },
        { typeof(TreeClusterBlock) }
    };

    public GameObject   spawnablePropsBlock;
  
    public RuleTile boundsRuletile;
    public RuleTile roadRuletile;
    public RuleTile grassRuletile;

    public GameObject boundsTilemapGO;
    public GameObject roadTilemapGO;

    [Range(0f, 1f)] public float clutterDensity = 0.3f;

    public LayerMask propsMask;

    public GameObject GetRandomProps(GameObject[] props)
    {
        if (props.Length > 0) return props[Random.Range(0, props.Length)];
        else return null;

    }
}
