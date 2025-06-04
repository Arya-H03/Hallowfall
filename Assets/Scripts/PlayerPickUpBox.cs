using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpBox : MonoBehaviour
{
    private PlayerPickUpHandler pickUpHandler;

    private void Awake()
    {
        pickUpHandler = GetComponentInParent<PlayerPickUpHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseItem item = collision.gameObject.GetComponent<BaseItem>();
        if (item)
        {
            pickUpHandler.ItemList.Remove(item);
            item.OnItemPickUp();
        }
    }
}
