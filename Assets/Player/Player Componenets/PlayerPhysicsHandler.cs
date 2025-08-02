using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPhysicsHandler : MonoBehaviour, IInitializeable<PlayerController>
{

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;

    public BoxCollider2D BoxCollider2D { get => boxCollider2D; private set => boxCollider2D = value; }
    void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void Init(PlayerController playerController)
    {
        rb = playerController.Rb;
    }

    public void KnockBackPlayer(Vector2 lanunchVector, float lunchForce)
    {
        StartCoroutine(KnockBackPlayerCoroutine(lanunchVector, lunchForce));
    }
    private IEnumerator KnockBackPlayerCoroutine(Vector2 lanunchVector, float force)
    {

        rb.linearVelocity += lanunchVector  * force;
        yield return new WaitForSeconds(0.25f);
        rb.linearVelocity -= lanunchVector  * force;


    }
   
}



