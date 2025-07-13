using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GraveClusterBlock : PropsBlock
{
    private ParticleSystem earthShakeParticleSystem;
    protected override void PopulateBlock(CellGrid cellGrid, ZoneLayoutProfile zoneLayoutProfile)
    {
        TilePaint grassTilePaint = new TilePaint { tilemap = ZoneManager.Instance.GroundOneTilemap, tileBase = zoneLayoutProfile.grassRuletile };
        if (zoneLayoutProfile is GraveYardLayoutProfile graveYardLayoutProfile)
        {

            for (int y = 1; y < cellGrid.CellPerCol; y += 3)
            {
                for (int x = 1; x < cellGrid.CellPerRow - 1; x += 2)
                {


                    TileBase graveStoneTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.graveStoneTiles, false);
                    if (graveStoneTilebase != null)
                    {
                        TilePaint tilePaintGravestone = new TilePaint { tilemap = ZoneManager.Instance.PropsTilemap, tileBase = graveStoneTilebase };
                        cellGrid.Cells[x, y].AddToTilePaints(tilePaintGravestone);
                        cellGrid.Cells[x, y].IsOccupied = true;
                    }

                    TryAddGraveDirt(graveYardLayoutProfile, new Vector2Int(x, y - 1), celLGrid);
                    TryAddSkulls(graveYardLayoutProfile, (Vector3Int)cellGrid.Cells[x, y - 1].CellPos);

                }
            }

            for (int j = 0; j < cellGrid.CellPerCol; j++)
            {
                for (int i = 0; i < cellGrid.CellPerRow; i++)
                {

                    cellGrid.Cells[i, j].AddToTilePaints(grassTilePaint);
                }
            }
        }

    }
    private void TryAddGraveDirt(GraveYardLayoutProfile graveYardLayoutProfile, Vector2Int cellCoord, CellGrid cellGrid)
    {
        if (Random.Range(1, 7) > 4)
        {

            TileBase graveDirtTilebase = graveYardLayoutProfile.GetRandomTile(graveYardLayoutProfile.graveDirtTiles, false);
            if (graveDirtTilebase != null)
            {
                cellGrid.Cells[cellCoord.x, cellCoord.y].IsOccupied = true;
                TilePaint tilePaintGravestone = new TilePaint { tilemap = ZoneManager.Instance.GroundTwoTilemap, tileBase = graveDirtTilebase };
                cellGrid.Cells[cellCoord.x, cellCoord.y].AddToTilePaints(tilePaintGravestone);
            }
        }
    }
    private void TryAddSkulls(GraveYardLayoutProfile graveYardLayoutProfile, Vector3 cellPos)
    {
        if (Random.value > 0.75)
        {
            int skullCount = Random.Range(1, 3);
            for (int i = 0; i < skullCount; i++)
            {
                Vector2 offset = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(-0.4f, 0.4f));
                Vector3 pos = cellPos + (Vector3)offset;
                GameObject skullGO = Instantiate(graveYardLayoutProfile.skullPrefab, pos, Quaternion.identity);
                skullGO.GetComponent<SpriteRenderer>().sprite = graveYardLayoutProfile.GetRandomSprite(graveYardLayoutProfile.skullSprites);
                skullGO.transform.parent = propsHolder.transform;
                skullGO.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f));
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("In");
            isPlayerOnThisBlock = true;
            StartCoroutine(SpawnEnemiesCoroutine());
        }



    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Out");
            isPlayerOnThisBlock = false;
        }

    }

   private IEnumerator SpawnEnemiesCoroutine()
    {
        GraveYardLayoutProfile graveYardLayout = zoneLayoutProfile as GraveYardLayoutProfile;
        if(!earthShakeParticleSystem)
        {
            earthShakeParticleSystem = Instantiate(graveYardLayout.groundShakeParticleEffectPrefab, transform.position, Quaternion.identity);
            earthShakeParticleSystem.transform.parent = this.transform;
        }
       


        yield return new WaitForSeconds(2f);
        while (isPlayerOnThisBlock)
        {

            int x = Random.Range(0, celLGrid.CellPerRow);
            int y = Random.Range(0, celLGrid.CellPerCol);

            if (!celLGrid.Cells[x, y].IsOccupied)
            {
                GameObject groundShakeEffect = celLGrid.TryInstantiateTempGameobjectOnTile(graveYardLayout.groundShakeEffectPrefab, new Vector2Int(x, y), Quaternion.identity);
                if (groundShakeEffect)
                {
                    earthShakeParticleSystem.transform.position = groundShakeEffect.transform.position; 
                    earthShakeParticleSystem.Play();
                    celLGrid.Cells[x, y].PaintCell(ZoneManager.Instance.GroundOneTilemap, graveYardLayout.defaultDirtTile);
                    yield return new WaitForSeconds(0.75f);
                    celLGrid.TryInstantiateTempGameobjectOnTile(EnemySpawnManager.Instance.SinnerPrefab, new Vector2Int(x, y), Quaternion.identity);
                }

                


            }

            yield return new WaitForSeconds(3f);
        }
    }


}
