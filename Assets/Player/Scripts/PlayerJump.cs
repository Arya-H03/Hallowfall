using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] float jumpSpeed;

    private float jumpDirectionX;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void StartJump()
    {
        jumpDirectionX = playerController.playerMovementManager.currentDirection.x;
        playerController.rb.gravityScale = 3;
        playerController.playerMovementManager.StopRunning();
        //playerController.animationController.SetBoolForAnimations("isRunning", false);
        //footSteps.OnEndPlayerFootstep();
        //footSteps.StopFootstepPSEffect();
        playerController.rb.velocity = new Vector2(jumpDirectionX * 5, jumpSpeed);
        playerController.animationController.SetBoolForAnimations("isJumping", true);
    }

    public void EndJump()
    {
        playerController.isPlayerGrounded = true;
        playerController.isPlayerJumping = false;
        playerController.canPlayerJump = true;
        playerController.animationController.SetBoolForAnimations("isJumping", false);
        if (playerController.playerMovementManager.currentDirection.x != 0)
        {
            playerController.playerMovementManager.StartRunning();
        }
    }
}
