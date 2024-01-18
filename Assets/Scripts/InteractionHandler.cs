using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnEnable()
    {
        PointOfInterest.OnPointOfInterestInteraction += HandlePointOfInterestInteraction;
    }

    private void OnDisable()
    {
        PointOfInterest.OnPointOfInterestInteraction -= HandlePointOfInterestInteraction;
    }

    private void HandlePointOfInterestInteraction(string interactionType)
    {
        
        switch (interactionType)
        {       
            case "Weapon":
                player.GetComponent<PlayerController>().HandelSwordEquipment(true);
                break;
            case "Dialoge":
                break;
               
        }
    }
}
