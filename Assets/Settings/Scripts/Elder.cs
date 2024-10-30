using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elder : MonoBehaviour
{
    [SerializeField]private GameObject interactIcon;
    private DialogueBox dialogueBox;
    private bool isPlayerInRange = false;

    private string[] onEnterRangeDialogues = {
        " He... is waiting for you",
        " Well Well Well...",
        " Help Him...",
    };
    private void Awake()
    {
        dialogueBox = GetComponentInChildren<DialogueBox>();
    }

    private void PlayRandomDialogue(string[] texts)
    {
        int randomIndex = Random.Range(0, texts.Length);
        //dialogueBox.StartDialouge(texts[randomIndex]);
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
                
            }
        }
    }
    private void ActivateInteractIcon()
    {
        interactIcon.SetActive(true);
        //dialogueBox.PlayRandomDialogue(onEnterRangeDialogues);
       
    }

    private void DeActivateInteractIcon()
    {
        interactIcon.SetActive(false);
    }
}
