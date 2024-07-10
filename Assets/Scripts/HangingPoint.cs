using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingPoint : MonoBehaviour
{
    GameObject player;
    [SerializeField] Transform hangingPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject; 
            player.GetComponent<PlayerCollision>().Rb.bodyType = RigidbodyType2D.Static;
            player.GetComponent<PlayerAnimationController>().SetBoolForAnimations("isHanging", true);
            

        }
    }

    private void Update()
    {
        if (player)
        {
            player.transform.position = hangingPoint.position;
        }

    }
}
