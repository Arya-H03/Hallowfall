using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialoge : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Color textColor;
    public string text;
    private float textSpeed = 0.1f;

    private float duration = 5f;

    private Image dialogeBox;
    private Color dialogeColor;

    private void Awake()
    {
        dialogeBox = GetComponent<Image>();
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        dialogeColor = dialogeBox.color;
        textColor = textComponent.color;
    }

    private void Start()
    {
        textComponent.text = string.Empty;
    }
    public void StartDialoge(string inputext)
    {
        text = inputext;
        dialogeBox.color = new Color(dialogeColor.r, dialogeColor.g, dialogeColor.b, 0.75f);
        textComponent.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        StartCoroutine(TypeLine());
    }

    private IEnumerator EndDialoge()
    {
        yield return new WaitForSeconds(5f);
        while (dialogeBox.color.a > 0f)
        {
            dialogeBox.color = new Color(dialogeColor.r, dialogeColor.g, dialogeColor.b, dialogeBox.color.a - (dialogeColor.a * textSpeed/ duration));
            textComponent.color = new Color(textColor.r, textColor.g, textColor.b, textComponent.color.a - (textColor.a * textSpeed / duration));
            yield return new WaitForSeconds(textSpeed);
        }
    }


    IEnumerator TypeLine()
    {
        int i = 0;
        foreach(char c in text.ToCharArray())
        {
            i++;    
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
            if (i == text.ToCharArray().Length)
            {
                StartCoroutine(EndDialoge());
            }
        }
    }

   
}
