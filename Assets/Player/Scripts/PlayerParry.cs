using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] GameObject parryShield;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    public void StartParry()
    {
        playerController.animationController.SetBoolForAnimations("isRunning", false);
        playerController.animationController.SetBoolForAnimations("isJumping", false);
        playerController.animationController.SetTriggerForAnimations("Parry");

        if (playerController.isPlayerJumping)
        {
            playerController.rb.gravityScale = 0.2f;
            playerController.rb.velocity = Vector2.zero;
        }

        playerController.isParrying = true;
    }

    public void ActivateParryShield()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void OnParryEnd()
    {
        
        parryShield.GetComponent<BoxCollider2D>().enabled = false;
        playerController.rb.gravityScale = 3;
        if (playerController.playerMovementManager.currentDirection.x != 0)
        {
            playerController.animationController.SetBoolForAnimations("isRunning", true);
        }
        playerController.isParrying = false;
    }
}
