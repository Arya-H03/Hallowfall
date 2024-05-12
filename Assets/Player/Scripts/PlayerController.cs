using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerAnimationController animationController;
    [HideInInspector]
    public PlayerMovementManager playerMovementManager;
    [HideInInspector]
    public PlayerJump playerJump;
    [HideInInspector]
    public PlayerAttacks playerAttacks;
    [HideInInspector]
    public PlayerParry playerParry;
    [HideInInspector]
    public Rigidbody2D rb;
    public InputManager inputManager;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public ParticleSystem deathEffectParticle;
    [HideInInspector]
    public Player player;



    public bool canPlayerJump = true;
    public bool isPlayerGrounded = true;
    public bool isPlayerJumping = false;
    public bool canPlayerAttack = true;
    public bool isParrying  = false;
    public bool hasSword = false;
    public bool isDead = false;
    public bool isAttacking = false;

    private void Awake()
    {     
        animationController = GetComponentInChildren<PlayerAnimationController>();
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerJump = GetComponentInChildren<PlayerJump>();
        playerAttacks = GetComponentInChildren<PlayerAttacks>();
        playerParry = GetComponent<PlayerParry>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathEffectParticle = GetComponent<ParticleSystem>();
        player = GetComponent<Player>();

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
            isPlayerGrounded = false;
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
            
            playerParry.StartParry();
        }
       
  
    }

    public void HandelSwordEquipment(bool isEquiped)
    {
        animationController.ChangeAnimatorAC(isEquiped);
        hasSword = isEquiped;
    }

    public void RestoreHealth()
    {

    }


}
