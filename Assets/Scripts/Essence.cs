using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
       StartCoroutine(LaunchEssence());
    }

    
    void Update()
    {
        
    }

    private IEnumerator LaunchEssence()
    {
        int xDir = Random.Range(-1, 2);
        rb.velocity = new Vector2(rb.velocity.x + xDir, rb.velocity.y + 7);
        yield return new WaitForSeconds(1);
        rb.velocity = new Vector2(rb.velocity.x - xDir, rb.velocity.y - 7);
    }
}
