using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private DialogueBox dialogueBox;

    public Vector2 currentDirection;

    [SerializeField] float speed;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (!playerController.isPlayerJumping && !playerController.player.isPlayerDead)
        {

            transform.position += new Vector3(currentDirection.x, 0,0) * speed * Time.deltaTime;

        }
    }

    private void OnPlayerTurning()
    {
        float scaleX = 1;

        if (currentDirection.x > 0)
        {
            scaleX = 1;
        }
        else if (currentDirection.x < 0)
        {
            scaleX = -1;
            
        }
        else if (currentDirection.x == 0)
        {
            scaleX = transform.localScale.x;
        }

        transform.localScale = new Vector3(scaleX, 1, 1);
        dialogueBox.transform.localScale = new Vector3(scaleX, 1, 1);
    }
    private void ManageRunState()
    {
        if (currentDirection.x != 0 && !playerController.isPlayerJumping)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
    }

    //public void StartRunning()
    //{
    //    playerController.AnimationController.SetBoolForAnimations("isRunning", true);
    //    footSteps.OnStartPlayerFootstep();
        
    //}

    //public void StopRunning()
    //{
    //    playerController.AnimationController.SetBoolForAnimations("isRunning", false);
    //    footSteps.OnEndPlayerFootstep();
       
    //}


    public void HandleMovement(Vector2 dir)
    {
        currentDirection = dir;
        OnPlayerTurning();
        ManageRunState();
    }
}
