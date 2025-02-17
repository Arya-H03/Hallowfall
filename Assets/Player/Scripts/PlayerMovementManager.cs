using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private DialogueBox dialogueBox;

    public Vector2 currentInputDir; // -1 = "A" +1 = "D" 0= None 
    private Vector2 currentDirection = new Vector2(1,0);

    private float moveSpeed;

    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public Vector2 CurrentDirection { get => currentDirection; set => currentDirection = value; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!playerController.IsPlayerJumping && !playerController.IsDead && !playerController.IsHanging /*&& !playerController.IsFalling*/)
        {

            transform.position += new Vector3(currentInputDir.x, currentInputDir.y, 0) * MoveSpeed * Time.deltaTime;

            Vector3 clampedPos = transform.position;
            clampedPos.z = Mathf.Clamp(clampedPos.z,-1,2);
            transform.position = clampedPos;

        }
    }

    public void TurnPlayer(Vector2 vec)
    {
        float scaleX = 1;

        if (vec.x > 0)
        {
            scaleX = 1;
        }
        else if (vec.x < 0)
        {
            scaleX = -1;

        }
        else if (vec.x == 0)
        {
            scaleX = transform.localScale.x;
        }

        currentDirection.x = scaleX;
        transform.localScale = new Vector3(scaleX, 1, 1);
        dialogueBox.transform.localScale = new Vector3(scaleX, 1, 1);
    }
   
    private void ManageRunState()
    {
        if (!playerController.IsPlayerJumping && !playerController.IsFalling && !playerController.IsHanging && !playerController.IsParrying && !playerController.IsAttacking && !playerController.IsRolling)
        {
            if (currentInputDir != Vector2.zero)
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
        currentInputDir = dir;
        TurnPlayer(currentInputDir);
        ManageRunState();
    }
}
