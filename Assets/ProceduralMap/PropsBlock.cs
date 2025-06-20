using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PropsBlock : MonoBehaviour
{
    [SerializeField] GameObject propsHolder;
    public List<Transform> children = new List<Transform>();

    public void RandomizeProps()
    {
        foreach(Transform child in propsHolder.transform)
        {
            children.Add(child);
            int rand = Random.Range(0, 2);
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if(rand == 0) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;
        }

    }
}
