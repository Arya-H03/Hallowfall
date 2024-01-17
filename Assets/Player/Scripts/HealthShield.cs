using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthShield : MonoBehaviour
{
    private Animator animator;
    private Image image;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }


    public void PlayBreakAnimation()
    {
        animator.SetTrigger("Break");
    }

    public void DeActivateHealthShield()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b,0f);
    }

    public void ReActivateHealthShield()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }
}
