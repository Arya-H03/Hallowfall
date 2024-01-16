using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialoge : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] text;
    private int index;
    private float textSpeed = 0.05f;


    private void Start()
    {
        textComponent.text = string.Empty;
        Invoke("StartDialoge", 0.5f);
        Invoke("OnDestroy", 10f);
    }
    private void StartDialoge()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach(char c in text[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (index < text.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine (TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
