using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InteractionHandler;

public class PointOfInterest : MonoBehaviour
{
    public enum InteractionTypeEnum
    {

        None,
        Weapon
    }


    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] GameObject interactIcon;
    [SerializeField] private InteractionTypeEnum interactionType;
    [SerializeField] private string dialogueText;
    [SerializeField] private bool hasDialouge;
    


    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ActivateInteractIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            DeActivateInteractIcon();
        }
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleInteraction();
            }
        }
    }
    private void ActivateInteractIcon()
    {
        interactIcon.SetActive(true);
        
        
    }

    private void DeActivateInteractIcon()
    {
        interactIcon.SetActive(false);
    }

    private void HandleInteraction()
    {
        if (hasDialouge)
        {
            //dialogueBox.StartDialouge(dialogueText);
        }

        switch (interactionType)
        {
            case InteractionTypeEnum.Weapon:
                GameObject player;
                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<PlayerController>().HandelSwordEquipment(true);
                break;
        }
    }

}
