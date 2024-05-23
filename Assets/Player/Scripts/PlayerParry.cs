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
        playerController.AnimationController.SetBoolForAnimations("isRunning", false);
        playerController.AnimationController.SetBoolForAnimations("isJumping", false);
        playerController.AnimationController.SetTriggerForAnimations("Parry");

        if (playerController.IsPlayerJumping)
        {
            playerController.rb.gravityScale = 0.2f;
            playerController.rb.velocity = Vector2.zero;
        }

        playerController.IsParrying = true;
    }

    public void ActivateParryShield()
    {
        parryShield.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void OnParryEnd()
    {
        
        parryShield.GetComponent<BoxCollider2D>().enabled = false;
        playerController.rb.gravityScale = 3;
        if (playerController.PlayerMovementManager.currentDirection.x != 0)
        {
            playerController.AnimationController.SetBoolForAnimations("isRunning", true);
        }
        playerController.IsParrying = false;
    }
}
