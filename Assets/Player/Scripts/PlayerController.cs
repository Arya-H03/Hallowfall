using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PlayerAnimationController animationController;
    public PlayerMovementManager playerMovementManager;
    public PlayerJump playerJump;
    public PlayerAttacks playerAttacks;
    public PlayerParry playerParry;
    public Rigidbody2D rb;



    public bool canPlayerJump = true;
    public bool isPlayerGrounded = true;
    public bool isPlayerJumping = false;
    public bool canPlayerAttack = true;
    public bool isParrying  = false;
    public bool hasSword = false;

    private void Awake()
    {     
        animationController = GetComponentInChildren<PlayerAnimationController>();
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerJump = GetComponent<PlayerJump>();
        playerAttacks = GetComponentInChildren<PlayerAttacks>();
        playerParry = GetComponent<PlayerParry>();
        rb = GetComponent<Rigidbody2D>();

    }
    public void OnMove(Vector2 dir)
    {
        playerMovementManager.HandleMovement(dir);
    }

    public void OnJumpStart()
    {
        if (isPlayerGrounded && canPlayerJump )
        {
            isPlayerJumping = true;
            canPlayerJump = false;
            canPlayerAttack = false;
            playerJump.StartJump();
        }     
    }

    public void OnStartAttack(int attackIndex)
    {
        if (hasSword)
        {
            playerAttacks.StartAttack(isPlayerJumping, attackIndex);
        }
        
    }

    public void OnParry()
    {
        if (hasSword)
        {
            isParrying = true;
            playerParry.StartParry();
        }
       
  
    }

    private void HandelSwordEquipment(bool isEquiped)
    {
        animationController.ChangeAnimatorAC(isEquiped);
        hasSword = isEquiped;
    }
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            HandelSwordEquipment(true);

        }


        if (Input.GetKeyDown(KeyCode.V))
        {
            HandelSwordEquipment(false);

        }

    }
}
