using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryShield : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] GameObject impactEffect;
    private PlayerController playerController;
    private AudioSource audioSource;
    public static event Action OnParrySuccessful;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        audioSource = GetComponentInParent<AudioSource>();
    }
    private void OnEnable()
    {
        OnParrySuccessful += CreateVisuallOnParrySucceess;
    }

    private void OnDisable()
    {
        OnParrySuccessful -= CreateVisuallOnParrySucceess;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        

    }

    public void CallOnParrySuccessfulEvent()
    {
        OnParrySuccessful?.Invoke();
       
    }

    private void CreateVisuallOnParrySucceess()
    {
        playerController.AnimationController.SetBoolForAnimations("isParrySuccessful", true);
        audioSource.Play();
    }
    public void SpawnImpactEffect(Vector3 position)
    {
        GameObject obj =Instantiate(impactEffect, position, Quaternion.identity);
        Destroy(obj,0.5f);
    }

    public bool CanCounter()
    {
        return playerController.PlayerParryState.CanCounterParry;
    }
}
