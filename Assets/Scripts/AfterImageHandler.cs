using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AfterImageHandler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject imagePrefab;
    [SerializeField] float imageLifeTime = 0.4f;
   

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        
    }
    public IEnumerator SpawnImage()
    {
        while (true)
        {
            GameObject afterImageGO = Instantiate(imagePrefab, this.transform.position,Quaternion.identity);
            afterImageGO.GetComponent<AfterImage>().InitializeImage(spriteRenderer.sprite, spriteRenderer.flipX,imageLifeTime,spriteRenderer.color,this.transform.localScale.x);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
