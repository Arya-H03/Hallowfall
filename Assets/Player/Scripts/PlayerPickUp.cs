using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Essence>() != null)
        {
            //collision.GetComponent<Essence>().isPlayerInRange = true;
            collision.GetComponent<Essence>().SnapToPlayer(this.gameObject);
        }
    }
}
