using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceRangeBox : MonoBehaviour
{
    private Essence essence;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        essence = GetComponentInParent<Essence>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            essence.isPlayerInRange = true;
            circleCollider.enabled = false;
            essence.canBePicked = true;
            

        }
       
    }


}
