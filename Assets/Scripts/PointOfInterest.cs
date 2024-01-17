using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class PointOfInterest : MonoBehaviour
{

    [SerializeField] GameObject intercatIcon;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ActivateInteractIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DeActivateInteractIcon();
        }
    }

    private void ActivateInteractIcon()
    {
        intercatIcon.SetActive(true);
    }

    private void DeActivateInteractIcon()
    {
        intercatIcon.SetActive(false);
    }
}
