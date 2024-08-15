using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;

    public BoxCollider2D BoxCollider2D { get => boxCollider2D; set => boxCollider2D = value; }
    public Rigidbody2D Rb { get => rb; set => rb = value; }

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        switch (collision.tag)
        {
            case "Trap":
                //playerController.PlayerCollision.Rb.bodyType = RigidbodyType2D.Static;
                break;
            case "Enemy":
                //rb.bodyType = RigidbodyType2D.Kinematic;
                //boxCollider2D.isTrigger = true;
                break;
        }
      
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //switch (collision.tag)
        //{
            
        //    //case "Enemy":
        //    //    rb.bodyType = RigidbodyType2D.Dynamic;
        //    //    boxCollider2D.isTrigger = false;
        //    //    break;
        //}
    }

    public void KnockPlayer(Vector2 launchVel)
    {
        playerController.rb.velocity += launchVel;
    }

  
}
