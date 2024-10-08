using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    private void Awake()
    {

        textComponent = GetComponentInChildren<TextMeshProUGUI>();

    }

    private void Start()
    {
        textComponent.text = string.Empty;
    }


    public void StartDialogue(string inputext, float delay)
    {
        ClearText();
        textComponent.text = inputext;
        StartCoroutine(EndDialogue(delay));

    }

    private IEnumerator EndDialogue(float delay)
    {

        yield return new WaitForSeconds(delay);
        ClearText();

    }

    public void ClearText()
    {
        textComponent.text = string.Empty;
    }
}
