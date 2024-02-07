using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private enum FloorType
    {
        ground,
        grass,
        wood
    }

    private FloorType currentFloorType = FloorType.ground;
    
    [SerializeField] GameObject runEffect;
    private ParticleSystem footstepPS;

    private PlayerController playerController;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] AudioClip groundClip;
    [SerializeField] AudioClip grassClip;
    [SerializeField] AudioClip woodClip;

    [SerializeField] GameObject rayCatPosition;

    private void Awake()
    {
        footstepPS = GetComponent<ParticleSystem>();
        playerController = GetComponentInParent<PlayerController>();
    }
    public void OnStartPlayerFootstep()
    {
        footstepAudioSource.Play();
        footstepPS.Play();
        //runEffect.SetActive(true);
    }

    public void OnEndPlayerFootstep()
    {
        footstepAudioSource.Stop();
        footstepPS.Stop();
        //runEffect.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerController.isPlayerJumping == true)
        {
            if (currentFloorType == FloorType.grass && collision.CompareTag("Grass"))
            {

                playerController.playerJump.EndJump();

            }

            else if(currentFloorType == FloorType.ground && collision.CompareTag("Ground"))
            {
                playerController.playerJump.EndJump();
            }

            else if (currentFloorType == FloorType.wood && collision.CompareTag("Wood"))
            {
                playerController.playerJump.EndJump();
            }


        }
    }
    private void CheckGround()
    {
        
        RaycastHit2D rayCast = Physics2D.Raycast(rayCatPosition.transform.position, Vector2.down,1f, groundLayer);
        Debug.DrawLine(rayCatPosition.transform.position, rayCatPosition.transform.position + Vector3.down * 1f, Color.red);

        if (rayCast)
        {
            if (currentFloorType == FloorType.ground)
            {
                if (rayCast.collider.CompareTag("Grass"))
                {
                    footstepAudioSource.Stop();
                    currentFloorType = FloorType.grass;
                    footstepAudioSource.clip = grassClip;
                    footstepAudioSource.Play();
                }
            }

            else if (currentFloorType == FloorType.grass)
            {
                if (rayCast.collider.CompareTag("Ground"))
                {
                    footstepAudioSource.Stop();
                    currentFloorType = FloorType.ground;
                    footstepAudioSource.clip = groundClip;
                    footstepAudioSource.Play();
                }
            }

            else if (currentFloorType == FloorType.wood)
            {
                if (rayCast.collider.CompareTag("Wood"))
                {
                    footstepAudioSource.Stop();
                    currentFloorType = FloorType.wood;
                    footstepAudioSource.clip = woodClip;
                    footstepAudioSource.Play();
                }
            }
        }
        
        
        
    }

    private void FixedUpdate()
    {
        CheckGround();
    }
}
