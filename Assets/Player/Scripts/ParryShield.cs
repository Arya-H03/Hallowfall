using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryShield : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] GameObject impactEffect;
    private PlayerController controller;
    private AudioSource audioSource;

    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
        audioSource = GetComponentInParent<AudioSource>();
        //playerRB = gameObject.GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Projectile"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = collision.gameObject.GetComponent<Rigidbody2D>().velocity * -1; 
            playerRB.velocity = new Vector2(playerRB.velocity.x - 3f, playerRB.velocity.y);
        }
    }

    public void OnSuccessfulParry()
    {
        controller.animationController.SetBoolForAnimations("isParrySuccessful", true);
        audioSource.Play();
    }

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj =Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj,0.5f);
    }
}
