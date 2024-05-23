using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;
    private AudioSource audioSource;

    [SerializeField] float jumpSpeed;

    [SerializeField] GameManager gameManager;

    private float jumpDirectionX;

    [SerializeField] AudioClip jumpUpAC;
    [SerializeField] AudioClip groundHitAC;

    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponentInParent<PlayerController>();
    }
    void Start()
    {
        
    }

    public void StartJump()
    {
        //Debug.Log("Start");
        jumpDirectionX = playerController.PlayerMovementManager.currentDirection.x;
        playerController.rb.gravityScale = 3;
        //playerController.PlayerMovementManager.StopRunning();
        playerController.rb.velocity = new Vector2(jumpDirectionX * 5, jumpSpeed);
        gameManager.PlayAudio(audioSource, jumpUpAC);
        //playerController.animationController.SetBoolForAnimations("isJumping", true);
    }

    public void EndJump()
    {
        //Debug.Log("End");
        playerController.isPlayerGrounded = true;
        playerController.isPlayerJumping = false;
        playerController.canPlayerJump = true;
        gameManager.PlayAudio(audioSource, groundHitAC);
        //playerController.animationController.SetBoolForAnimations("isJumping", false);
        if (playerController.PlayerMovementManager.currentDirection.x != 0)
        {
            //playerController.PlayerMovementManager.StartRunning();
        }
    }
}
