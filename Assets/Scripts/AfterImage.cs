using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private float lifeTime;
    Color color;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        StartCoroutine(FadeImageCoroutine());
    }
    public void InitializeImage(Sprite sprite, bool flipBool,float lifeTime,Color color,float xScale)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = flipBool;
        Vector3 localScale = transform.localScale;
        if(xScale <0)
        {
            localScale = new Vector3 (-localScale.x, localScale.y, localScale.z);    
        } 
        else localScale = new Vector3(localScale.x, localScale.y, localScale.z);

        this.transform.localScale = localScale;
        this.transform.position -= new Vector3(0, spriteRenderer.size.y / 4,0);

        this.color = color;
        this.lifeTime = lifeTime;
        spriteRenderer.color = color;
    }

    IEnumerator FadeImageCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lifeTime)
        {
            float alpha = Mathf.Lerp(color.a, 0f, elapsedTime / lifeTime);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);

    }
}
