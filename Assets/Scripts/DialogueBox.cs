using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private string text;
    [SerializeField] private float textSpeed = 0.05f;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();  
    }

    private void Start()
    {
       
    }
    public void StartDialouge(string inputText)
    {
        StartCoroutine(ClearText(0));
        StartCoroutine(TypeLetters(inputText));
    }

    private IEnumerator ClearText(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.text = string.Empty;
        textComponent.text = string.Empty;
    }
     private IEnumerator TypeLetters(string inputText)
     {
        this.text = inputText;
        char[] textChar = this.text.ToCharArray();

        for(int i = 0; i< textChar.Length; i++)
        {          
            textComponent.text += textChar[i];
            yield return new WaitForSeconds(textSpeed);
            if (i == textChar.Length -1)
            {
                StartCoroutine(ClearText(4));
            }
        }
     
     }

    public void PlayRandomDialogue(string [] dialogues)
    {
        int randomIndex = Random.Range(0, dialogues.Length);
        StartDialouge(dialogues[randomIndex]);
    }
}
