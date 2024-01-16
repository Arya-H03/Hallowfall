using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textBox;

    private float lifeTime = 1;

    private Color textColor;

    private void Awake()
    {
        textBox = GetComponent<TextMeshPro>();
        textColor = textBox.color;
    }

    public void SetText(string text)
    {
        textBox.text = text;
    }

    private void Update()
    {
        transform.position += new Vector3(0, 1) * 2 * Time.deltaTime;
        
        lifeTime -= Time.deltaTime; 
        if(lifeTime < 0)
        {         
            textColor.a -= 5 * Time.deltaTime;  
            textBox.color = textColor;
            if(textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
