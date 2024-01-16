using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log(collision.gameObject.GetComponent<Rigidbody2D>());
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0);
        }
    }
}
