using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GraveClusterBlock : PropsBlock
{
    private ParticleSystem earthShakeParticleSystem;
    protected override void PopulateBlock(SubCellGrid subCellGrid)
    {
        //CellPaint grassTilePaint = new CellPaint { tilemap = zoneHandler.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile, isOnGlobalTile = false };
        for (int y = 1; y < subCellGrid.CellPerCol; y += 3)
        {
            for (int x = 1; x < subCellGrid.CellPerRow - 1; x += 2)
            {


                TileBase graveStoneTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.graveStoneTiles, false);
                if (graveStoneTilebase != null)
                {
                    CellPaint tilePaintGravestone = new CellPaint {tilemap = zoneHandler.PropsWithCollisionTilemap, tileBase = graveStoneTilebase };
                    subCellGrid.Cells[x, y].AddToCellPaint(tilePaintGravestone);
                    subCellGrid.Cells[x, y].MarkAsOccupied();
                    subCellGrid.Cells[x, y].MarkAsUnwalkable();
                }

                TryAddGraveDirt(zoneLayoutProfile, new Vector2Int(x, y - 1), subCellGrid);
               

            }
        }

        base.SubCellGrid.LoopOverGrid((i, j) =>
        {
            if (!subCellGrid.Cells[i, j].IsOccupied) TryAddSkulls(zoneLayoutProfile, base.SubCellGrid.Cells[i, j]);
            //subCellGrid.Cells[i, j].AddToCellPaint(grassTilePaint);
        });

      
        subCellGrid.AddToBlockPaints(zoneHandler.GroundOneTilemap, zoneLayoutProfile.grassRuletile, subCellGrid,subCellGrid.ParentCellGrid);


    }
    private void TryAddGraveDirt(ZoneLayoutProfile zoneLayoutProfile, Vector2Int cellCoord, SubCellGrid subCellGrid)
    {
        if (Random.Range(1, 7) > 4)
        {

            TileBase graveDirtTilebase = zoneLayoutProfile.GetRandomTile(zoneLayoutProfile.graveDirtTiles, false);
            if (graveDirtTilebase != null)
            {
                //subCellGrid.Cells[cellCoord.x, cellCoord.y].IsOccupied = true;
                CellPaint tilePaintGravestone = new CellPaint { tilemap = zoneHandler.GroundTwoTilemap, tileBase = graveDirtTilebase };
                subCellGrid.Cells[cellCoord.x, cellCoord.y].AddToCellPaint(tilePaintGravestone);
            }
        }
    }
    private void TryAddSkulls(ZoneLayoutProfile zoneLayoutProfile, Cell cell)
    {
        if (Random.value > 0.7)
        {
            TileBase skullTile = MyUtils.GetRandomRef(zoneLayoutProfile.skullTiles);
            CellPaint skullTilePaint = new CellPaint { tilemap = zoneHandler.PropsNoCollisionTilemap, tileBase = skullTile };
            cell.AddToCellPaint(skullTilePaint);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //PlayerCameraHandler.Instance.isInFog = true;
            isPlayerOnThisBlock = true;
            ////StartCoroutine(SpawnEnemiesCoroutine());
            //PlayerCameraHandler.Instance.EnableFog(3);
            StartCoroutine(SpawnEnemiesCoroutine());
            
        }



    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //PlayerCameraHandler.Instance.isInFog = true;
           

        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //PlayerCameraHandler.Instance.isInFog = false;
            isPlayerOnThisBlock = false;
            //PlayerCameraHandler.Instance.DisableFog(3);
        }

    }

   private IEnumerator SpawnEnemiesCoroutine()
    {
        if (!earthShakeParticleSystem)
        {
            earthShakeParticleSystem = Instantiate(zoneLayoutProfile.groundShakeParticleEffectPrefab, transform.position, Quaternion.identity);
            earthShakeParticleSystem.transform.parent = this.transform;
        }

        //yield return new WaitForSeconds(2f);
        while (isPlayerOnThisBlock)
        {

            int x = Random.Range(0, SubCellGrid.CellPerRow);
            int y = Random.Range(0, SubCellGrid.CellPerCol);

            if (!SubCellGrid.Cells[x, y].IsOccupied)
            {
                GameObject groundShakeEffect = SubCellGrid.TryInstantiateTempGameobjectOnTile(zoneLayoutProfile.groundShakeEffectPrefab, new Vector2Int(x, y), Quaternion.identity);
                if (groundShakeEffect)
                {
                    earthShakeParticleSystem.transform.position = groundShakeEffect.transform.position; 
                    earthShakeParticleSystem.Play();
                    //subCellGrid.Cells[x, y].PaintCell(ZoneManager.Instance.GroundOneTilemap, graveYardLayout.defaultDirtTile);
                    yield return new WaitForSeconds(0.1f);
                    SubCellGrid.TryInstantiateTempGameobjectOnTile(EnemySpawnManager.Instance.SinnerPrefab, new Vector2Int(x, y), Quaternion.identity);
                }

             

            }

            yield return new WaitForSeconds(10f);
        }
    }


}
