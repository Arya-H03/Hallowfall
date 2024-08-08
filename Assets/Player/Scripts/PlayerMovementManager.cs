using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private DialogueBox dialogueBox;

    public Vector2 currentDirection;

    private float moveSpeed;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!playerController.IsPlayerJumping && !playerController.Player.isPlayerDead && !playerController.IsHanging)
        {

            transform.position += new Vector3(currentDirection.x, 0,0) * MoveSpeed * Time.deltaTime;

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
        if (!playerController.IsPlayerJumping && !playerController.IsFalling && !playerController.IsHanging)
        {
            if (currentDirection.x != 0 )
            {

                playerController.ChangeState(PlayerStateEnum.Run);
            }
            else
            {
                playerController.ChangeState(PlayerStateEnum.Idle);
            }
        }
       
    }
    public void HandleMovement(Vector2 dir)
    {
        currentDirection = dir;
        OnPlayerTurning();
        ManageRunState();
    }
}
