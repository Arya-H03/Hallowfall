using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerFootSteps : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private enum FloorType
    {
        Ground,
        Grass,
        Wood
    }

    private FloorType currentFloorType = FloorType.Ground;

    [SerializeField] GameObject runEffect;
    private ParticleSystem footstepPS;

    private PlayerController playerController;

    [SerializeField] LayerMask groundLayer;

    private float footstepStartTime;

    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] AudioClip groundClip;
    [SerializeField] AudioClip grassClip;
    [SerializeField] AudioClip woodClip;

    [SerializeField] GameObject rayCatPosition;

    [SerializeField] private string[] floorTags = { "Ground", "Grass", "Wood" };

    private Dictionary<string, (FloorType floorType, AudioClip clip)> floorTypeMapping;

    private void Awake()
    {
        footstepPS = GetComponent<ParticleSystem>();
        playerController = GetComponentInParent<PlayerController>();

        // Initialize the floor type mapping
        floorTypeMapping = new Dictionary<string, (FloorType, AudioClip)>
        {
            { "Grass", (FloorType.Grass, grassClip) },
            { "Ground", (FloorType.Ground, groundClip) },
            { "Wood", (FloorType.Wood, woodClip) }
        };
    }

    private void FixedUpdate()
    {
        GroundCheckForFloorType();
        EndingJumpGroundCheck();
    }
    public void OnStartPlayerFootstep()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.time = footstepStartTime;
            footstepAudioSource.Play();
            footstepPS.Play();
        }
    }

    public void OnEndPlayerFootstep()
    {
        footstepStartTime = footstepAudioSource.time;
        footstepAudioSource.Stop();
        footstepPS.Stop();
    }

    private void EndingJumpGroundCheck()
    {
        if (playerController.IsPlayerJumping && playerController.rb.velocity.y < 0)
        {
            RaycastHit2D rayCast = Physics2D.Raycast(rayCatPosition.transform.position, Vector2.down, 0.25f, groundLayer);
            //Debug.DrawLine(rayCatPosition.transform.position, rayCatPosition.transform.position + Vector3.down * 0.1f, Color.red);
            if (rayCast)
            {
                
                foreach (string tag in floorTags)
                {
                    
                    if (rayCast.collider.CompareTag(tag))
                    {
                        
                        playerController.AnimationController.SetBoolForAnimations("isFalling", false);
                        playerController.PlayerJumpState.EndJump();
                        return;
                    }
                }
            }
        }
    }

    private void GroundCheckForFloorType()
    {
        RaycastHit2D rayCast = Physics2D.Raycast(rayCatPosition.transform.position, Vector2.down, 1f, groundLayer);
        // Debug.DrawLine(rayCatPosition.transform.position, rayCatPosition.transform.position + Vector3.down * 1f, Color.red);

        if (rayCast)
        {
            var tag = rayCast.collider.tag;
            if (floorTypeMapping.TryGetValue(tag, out var floorInfo))
            {
                if (currentFloorType != floorInfo.floorType)
                {
                    footstepAudioSource.Stop();
                    footstepStartTime = 0;
                    currentFloorType = floorInfo.floorType;
                    footstepAudioSource.clip = floorInfo.clip;
                    footstepAudioSource.Play();
                }
            }
        }
    }

   
}
