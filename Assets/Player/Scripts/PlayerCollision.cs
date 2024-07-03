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


        if (collision.CompareTag("Mist"))
        {

            //playerController.player.OnPlayerDeath();
            playerController.Player.OnPlayerDeath();

        }




    }

    public void KnockPlayer(Vector2 launchVel)
    {
        playerController.rb.velocity += launchVel;
    }

}
