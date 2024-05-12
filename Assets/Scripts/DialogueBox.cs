using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    [SerializeField] private string text;
    private float textSpeed = 0.05f;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();  
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    public string GetText()
    {
        return this.text;
    }

    public void StartDialouge()
    {

    }

     private IEnumerator TypeLetters()
     {
        int i = 0;
        foreach (char c in text.ToCharArray())
        {
            i++;
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
            if (i == text.ToCharArray().Length)
            {
                //End 
            }
        }
     }
}
