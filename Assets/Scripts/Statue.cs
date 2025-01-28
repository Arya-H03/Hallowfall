using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Statue : MonoBehaviour, IInteractable
{
    
    [SerializeField] private Transform respawnTransform;

    [SerializeField] private GameObject interactionCanvas;

    [SerializeField] private GameObject purpleFire;

    private AudioSource audioSource;
    [SerializeField] private AudioClip activationAC;

    private void Awake()
    {
      
        audioSource = GetComponent<AudioSource>();
    }
    public Transform GetStatueRespawnPoint()
    {
        return respawnTransform;
    }

    public void OnIntercationBegin()
    {
        interactionCanvas.SetActive(true);  
        InputManager.Instance.InputActions.Guardian.Interact.performed += Interact;
    }

    public void OnIntercationEnd()
    {
        InputManager.Instance.InputActions.Guardian.Interact.performed -= Interact;
        interactionCanvas.SetActive(false);
    }

    public void SetPurpleFire(bool shouldBeActive)
    {
        purpleFire.SetActive(shouldBeActive);
    }
    public void Interact(InputAction.CallbackContext ctx)
    {
        if(GameManager.Instance.LastStatue != this)
        {
            if (GameManager.Instance.LastStatue)
            {
                GameManager.Instance.LastStatue.SetPurpleFire(false);
            }
            
            GameManager.Instance.LastStatue = this;
            SetPurpleFire(true);
            AudioManager.Instance.PlaySFX(audioSource, activationAC);
            StartCoroutine(GameManager.Instance.SpawnEnemies());
            
        }
       
    }
}
