using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Statue : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform respawnTransform;

    [SerializeField] private GameObject interactionCanvas;

    [SerializeField] private GameObject purpleFire;

    
    
    public void SetPlayerPositionOnRespawn(GameObject player)
    {
        player.transform.position = respawnTransform.position;
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
        }
       
    }
}
