using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Atonement>() != null)
        {
   
            collision.GetComponent<Atonement>().SnapToPlayer(this.gameObject);
        }
    }
}
