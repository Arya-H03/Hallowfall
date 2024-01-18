using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    private GameObject dialogeBox;
    [SerializeField] GameObject interactIcon;

    public string interactionType;
    public string dialogeText;

    private bool isPlayerInRange = false;
   
    public delegate void InteractionEventHandler(string interactionType);
    public static event InteractionEventHandler OnPointOfInterestInteraction;

    private void Awake()
    {
        dialogeBox = GameObject.FindGameObjectWithTag("DialogeBox");
    }

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
                Interaction();
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

    private void Interaction()
    {
       
        if (OnPointOfInterestInteraction != null)
        {
            if(dialogeText != "")
            {
                dialogeBox.GetComponent<Dialoge>().StartDialoge(dialogeText);
            }
           
            OnPointOfInterestInteraction(interactionType);
        }
    }
}
