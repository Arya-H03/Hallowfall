using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestBlockState : BaseBlockInteractionState
{
    private PlayerController playerController;
    private Tilemap treeTilemap;
    private readonly int detectionRadius = 4;
    private readonly int detectionRadiusSqr;
    private Color vignetteColor;
    private CCoroutineRunner coroutineRunner;

    private HashSet<Vector3Int> fadedTiles = new();
    private HashSet<Vector3Int> tilesToFade = new();
    private HashSet<Vector3Int> tilesToUnfade = new();
    private Dictionary<Vector3Int, Coroutine> fadingTilesDict = new();

    public ForestBlockState(PlayerController playerController, CCoroutineRunner coroutineRunner, BlockTypeEnum blockType, Tilemap treeTilemap, int detectionRadius, Color vignetteColor) : base(playerController, coroutineRunner, blockType)
    {
        this.coroutineRunner = coroutineRunner;
        this.blockType = blockType;
        this.treeTilemap = treeTilemap;
        this.playerController = playerController;
        this.detectionRadius = detectionRadius;
        this.vignetteColor = vignetteColor;

        detectionRadiusSqr = detectionRadius * detectionRadius;
    }

    public override void OnEnterBlock()
    {

        //playerController.PlayerSignalHub.OnVignette?.Invoke(0.9f, vignetteColor);
    }

    public override void OnExitBlock()
    {
      
        //playerController.PlayerSignalHub.OnVignette?.Invoke(0, Color.white);
    }

    public override void OnStayBlock()
    {
        TryDetectForest();
    }

    public void TryDetectForest()
    {
        if (!treeTilemap) return;
        Vector3 playerPos = playerController.GetPlayerPos() - new Vector3(0, 2, 0);
        CheckForNearbyTreeTiles(playerPos);
        HandleTiles(playerPos);

    }

    private void CheckForNearbyTreeTiles(Vector3 centerPos)
    {
        Vector3Int centerTilePos = new Vector3Int((int)centerPos.x, (int)centerPos.y, (int)centerPos.z);
        int count = 0;
        for (int i = -detectionRadius; i < detectionRadius; i++)
        {
            for (int j = -detectionRadius; j < detectionRadius; j++)
            {
                Vector3Int tilePos = centerTilePos + new Vector3Int(i, j, 0);
                if (!treeTilemap.HasTile(tilePos)) continue;

                count++;
                if (!fadedTiles.Contains(tilePos)) tilesToFade.Add(tilePos);

            }
        }
    }
    private void HandleTiles(Vector3 centerPos)
    {
        // Fade tiles that should be faded
        foreach (Vector3Int tilePos in tilesToFade)
        {
            if (fadedTiles.Contains(tilePos)) continue;

            if ((centerPos - (Vector3)tilePos).sqrMagnitude <= detectionRadiusSqr)
            {
                StartFade(tilePos, 0.2f, 0.3f);
                fadedTiles.Add(tilePos);
            }
        }


        tilesToFade.Clear();


        // Unfade tiles that left the radius
        foreach (Vector3Int tilePos in fadedTiles)
        {
            if ((centerPos - (Vector3)tilePos).sqrMagnitude > detectionRadiusSqr)
            {
                tilesToUnfade.Add(tilePos);
            }
        }

        foreach (Vector3Int tilePos in tilesToUnfade)
        {
            StartFade(tilePos, 1f, 0.2f);
            fadedTiles.Remove(tilePos);
        }

        tilesToUnfade.Clear();
    }

    private void StartFade(Vector3Int tilePos, float targetAlpha, float duration)
    {
        if (fadingTilesDict.TryGetValue(tilePos, out Coroutine coroutine)) coroutineRunner.StopCoroutine(coroutine);

        fadingTilesDict[tilePos] = coroutineRunner.StartCoroutine(FadeTileCoroutine(treeTilemap, tilePos, targetAlpha, duration));
    }

    private IEnumerator FadeTileCoroutine(Tilemap tilemap, Vector3Int tilePos, float targetAlpha, float duration)
    {
        Color startColor = tilemap.GetColor(tilePos);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            tilemap.SetColor(tilePos, Color.Lerp(startColor, endColor, t));
            yield return null;
        }

        tilemap.SetColor(tilePos, endColor);
        fadingTilesDict.Remove(tilePos);
    }
}
