using System.Collections;
using UnityEngine;

public class OpenFieldHandler : ZoneHandler
{
    protected override IEnumerator GenerateZoneCoroutine()
    {

        PopulateZoneWithPropBlocks(celLGrid, zoneLayoutProfile);
        yield return null;
        AddDefaultGroundTileForZone(zoneLayoutProfile);
        yield return null;
        StartCoroutine(celLGrid.PaintAllCellsCoroutine());
        yield return null;
        //ZoneManager.Instance.navMeshSurface.BuildNavMesh();


    }
}
