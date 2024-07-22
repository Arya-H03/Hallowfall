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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

    }

    public void OnSuccessfulParry()
    {
        controller.AnimationController.SetBoolForAnimations("isParrySuccessful", true);
        audioSource.Play();
    }

    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj =Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj,0.5f);
    }
}
