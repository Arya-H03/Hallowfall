using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb;
    private PlayerController playerController;

    [SerializeField] LayerMask GroundLayer;
    void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerController.isPlayerJumping == true)
        {
            if (collision.CompareTag("Ground"))
            {
                playerController.playerJump.EndJump();
            }
        }
       

    }
}
