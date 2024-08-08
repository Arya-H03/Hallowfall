using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTrigger: MonoBehaviour
{
    private bool isBridgeBroken = false;
    [SerializeField] private GameObject bridge;
    
    [SerializeField] private Sprite brokenBridge;
    [SerializeField] private Sprite Bridge;
    private SpriteRenderer currentSpriterenderer;
    private AudioSource audioSource;

    private BoxCollider2D bridgeCollider;

    private void Awake()
    {
        currentSpriterenderer = bridge.GetComponent<SpriteRenderer>();
        bridgeCollider = bridge.GetComponent<BoxCollider2D>();
        audioSource = bridge.GetComponent<AudioSource>();
        
    }

    private void OnEnable()
    {
        PlayerDeathState.PlayerRespawnEvent += ResetBridge;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isBridgeBroken)
        {
            BreakBridge();
        }
    }

    public void BreakBridge()
    {
        isBridgeBroken = true;
        currentSpriterenderer.sprite = brokenBridge;
        bridgeCollider.enabled = false;
        audioSource.Play();
    }

    private void ResetBridge()
    {
        isBridgeBroken = false;
        currentSpriterenderer.sprite = Bridge;
        bridgeCollider.enabled = true;
    }
}
