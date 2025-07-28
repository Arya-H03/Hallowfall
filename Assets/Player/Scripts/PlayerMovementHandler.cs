using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private ZoneManager zoneManager;
    private FlowFieldManager flowFieldManager;
    private float flowFieldGenerationDelay = 0.3f;
    private float flowFieldGenerationTimer = 0.3f;

    public Vector2 currentInputDir; // -1 = "A" +1 = "D" 0= None 
    private Vector2 currentDirection = new Vector2(1,0);
    private float speedModifer = 1;

    private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public Vector2 CurrentDirection { get => currentDirection; set => currentDirection = value; }
    public float SpeedModifer { get => speedModifer; set => speedModifer = value; }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        zoneManager = ZoneManager.Instance;
        flowFieldManager = FlowFieldManager.Instance;   
        rb = playerController.PlayerPhysicsController.Rb;
    }

    private void Update()
    {
        if (zoneManager && flowFieldGenerationTimer >= flowFieldGenerationDelay)
        {
            flowFieldManager.UpdateFlowField(playerController.transform.position);
            flowFieldGenerationTimer = 0;

        }
        flowFieldGenerationTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if(playerController.CurrentStateEnum != PlayerStateEnum.Dash && playerController.CurrentStateEnum != PlayerStateEnum.Roll)
        {
            Vector2 targetPos = rb.position + (MoveSpeed * speedModifer * Time.deltaTime * currentInputDir);
            Vector2 smoothPos = Vector2.Lerp(rb.position, targetPos, 1f);
            rb.MovePosition(smoothPos);
        }
        //playerController.transform.position += new Vector3(currentInputDir.x, currentInputDir.y, 0) * MoveSpeed * speedModifer * Time.deltaTime;
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
        //dialogueBox.transform.localScale = new Vector3(scaleX, 1, 1);
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

    public void TurnPlayerWithMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (mousePos.x - transform.position.x > 0f)
        {
            TurnPlayer(Vector2.right);
        }

        if (mousePos.x - transform.position.x < 0f)
        {
            TurnPlayer(Vector2.left);
        }
    }
}
