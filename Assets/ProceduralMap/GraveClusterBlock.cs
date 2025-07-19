using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GraveClusterBlock : PropsBlock
{
    private ParticleSystem earthShakeParticleSystem;
    protected override void PopulateBlock(CellGrid cellGrid)
    {
        TilePaint grassTilePaint = new TilePaint { tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = true };
        for (int y = 1; y < cellGrid.CellPerCol; y += 3)
        {
            for (int x = 1; x < cellGrid.CellPerRow - 1; x += 2)
            {


                TileBase graveStoneTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.graveStoneTiles, false);
                if (graveStoneTilebase != null)
                {
                    TilePaint tilePaintGravestone = new TilePaint {tilemap = zoneHandler.PropsWithCollisionTilemap, tileBase = graveStoneTilebase };
                    cellGrid.Cells[x, y].AddToTilePaints(tilePaintGravestone);
                    cellGrid.Cells[x, y].IsOccupied = true;
                }

                TryAddGraveDirt(zoneLayoutProfile, new Vector2Int(x, y - 1), celLGrid);
               

            }
        }

        celLGrid.LoopOverGrid((i, j) =>
        {
            if (!cellGrid.Cells[i, j].IsOccupied) TryAddSkulls(zoneLayoutProfile, celLGrid.Cells[i, j]);
            cellGrid.Cells[i, j].AddToTilePaints(grassTilePaint);
        });
       
    }
    private void TryAddGraveDirt(ZoneLayoutProfile zoneLayoutProfile, Vector2Int cellCoord, CellGrid cellGrid)
    {
        if (Random.Range(1, 7) > 4)
        {

            TileBase graveDirtTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.graveDirtTiles, false);
            if (graveDirtTilebase != null)
            {
                cellGrid.Cells[cellCoord.x, cellCoord.y].IsOccupied = true;
                TilePaint tilePaintGravestone = new TilePaint { tilemap = zoneHandler.GroundTwoTilemap, tileBase = graveDirtTilebase };
                cellGrid.Cells[cellCoord.x, cellCoord.y].AddToTilePaints(tilePaintGravestone);
            }
        }
    }
    private void TryAddSkulls(ZoneLayoutProfile zoneLayoutProfile, Cell cell)
    {
        if (Random.value > 0.7)
        {
            TileBase skullTile = MyUtils.GetRandomRef(zoneLayoutProfile.skullTiles);
            TilePaint skullTilePaint = new TilePaint { tilemap = zoneHandler.PropsNoCollisionTilemap, tileBase = skullTile };
            cell.AddToTilePaints(skullTilePaint);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            isPlayerOnThisBlock = true;
            StartCoroutine(SpawnEnemiesCoroutine());
        }



    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            isPlayerOnThisBlock = false;
        }

    }

   private IEnumerator SpawnEnemiesCoroutine()
    {
        if(!earthShakeParticleSystem)
        {
            earthShakeParticleSystem = Instantiate(zoneLayoutProfile.groundShakeParticleEffectPrefab, transform.position, Quaternion.identity);
            earthShakeParticleSystem.transform.parent = this.transform;
        }

        yield return new WaitForSeconds(2f);
        while (isPlayerOnThisBlock)
        {

            int x = Random.Range(0, celLGrid.CellPerRow);
            int y = Random.Range(0, celLGrid.CellPerCol);

            if (!celLGrid.Cells[x, y].IsOccupied)
            {
                GameObject groundShakeEffect = celLGrid.TryInstantiateTempGameobjectOnTile(zoneLayoutProfile.groundShakeEffectPrefab, new Vector2Int(x, y), Quaternion.identity);
                if (groundShakeEffect)
                {
                    earthShakeParticleSystem.transform.position = groundShakeEffect.transform.position; 
                    earthShakeParticleSystem.Play();
                    //celLGrid.Cells[x, y].PaintCell(ZoneManager.Instance.GroundOneTilemap, graveYardLayout.defaultDirtTile);
                    yield return new WaitForSeconds(0.1f);
                    celLGrid.TryInstantiateTempGameobjectOnTile(EnemySpawnManager.Instance.SinnerPrefab, new Vector2Int(x, y), Quaternion.identity);
                }

             

            }

            yield return new WaitForSeconds(3f);
        }
    }


}
