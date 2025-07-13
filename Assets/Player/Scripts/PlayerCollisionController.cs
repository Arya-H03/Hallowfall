using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCollisionController : MonoBehaviour
{
    private PlayerController playerController;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;


    public BoxCollider2D BoxCollider2D { get => boxCollider2D; private set => boxCollider2D = value; }
    public Rigidbody2D Rb { get => rb; private set => rb = value; }


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

  
    public void KnockBackPlayer(Vector2 lanunchVector, float lunchForce)
    {
        StartCoroutine(KnockBackPlayerCoroutine(lanunchVector, lunchForce));
    }
    private IEnumerator KnockBackPlayerCoroutine(Vector2 lanunchVector, float force)
    {
       
        Rb.linearVelocity += lanunchVector  * force;
        yield return new WaitForSeconds(0.25f);
        Rb.linearVelocity -= lanunchVector  * force;


    }
    //private void CheckIfPlayerOnTreeTilemap(Tilemap tilemap)
    //{
    //    Vector3Int playerCurrentCell = tilemap.WorldToCell(playerController.GetPlayerCenter());
    //    bool onTreeTile = tilemap.GetTile(playerCurrentCell) != null;

    //    //Enters Forest
    //    if (onTreeTile && !isInForest)
    //    {
    //        OnEnterForest(tilemap);
    //    }
    //    //Exists Forest
    //    else if (!onTreeTile && isInForest)
    //    {
    //        OnExitForest();  
    //    }
    //}
  

}



