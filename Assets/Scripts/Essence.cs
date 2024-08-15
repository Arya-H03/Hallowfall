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

    private IEnumerator LaunchEssence()
    {
        float xDir = Random.Range(-3, 3.1f);
        rb.velocity = new Vector2(rb.velocity.x + xDir, rb.velocity.y + 7);
        yield return new WaitForSeconds(1.5f);
        rb.velocity = new Vector2(rb.velocity.x - xDir, rb.velocity.y - 7);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerPickUpBox"))
        {
            Destroy(this.gameObject);
        }
        
    }

    public void SnapToPlayer(GameObject player )
    {
        rb.velocity = new Vector2(player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y) * 2;
    }

}
