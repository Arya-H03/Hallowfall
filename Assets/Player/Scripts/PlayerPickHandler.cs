using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpHandler : MonoBehaviour
{
    private List<BaseItem> itemList = new List<BaseItem>();
    private PlayerController playerController;

    public List<BaseItem> ItemList { get => itemList; set => itemList = value; }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseItem item = collision.gameObject.GetComponent<BaseItem>();
        if (item)
        {
            itemList.Add(item);
        }
    }

    private void Update()
    {
        if(itemList.Count > 0)
        {
            foreach (BaseItem item in itemList)
            {
                if (item)
                {
                    Vector3 dir = (playerController.GetPlayerPos() - item.transform.position).normalized;
                    item.transform.position += dir * Time.deltaTime * 6f;
                }
               
            }
        }
        
    }
}
