using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController playerController;

    
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
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
