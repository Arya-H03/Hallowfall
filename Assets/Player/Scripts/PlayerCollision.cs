using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;

    //[SerializeField] LayerMask groundLayer;
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerController.isPlayerJumping == true)
        {
            if (collision.CompareTag("Ground"))
            {

                playerController.playerJump.EndJump();

            }


        }

        if (collision.CompareTag("Mist"))
        {

            playerController.player.OnPlayerDeath();

        }




    }

    //private void CheckPlayerIsGrounded()
    //{
    //    RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
    //    if (raycastHit2D.collider)
    //    {
    //        playerController.isPlayerGrounded = true;
    //    }
    //    else
    //    {
    //        playerController.isPlayerGrounded = false;
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    CheckPlayerIsGrounded();
    //}
}
