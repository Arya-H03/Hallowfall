using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : BaseItem
{
    [SerializeField] Sprite[] sprites;
  
    public override void OnItemPickUp()
    {
        GameManager.Instance.AddToPlayerSkulls(1);
        ObjectPoolManager.Instance.SkullPool.ReturnToPool(gameObject);
    }

    public override void OnItemDrop()
    {
        if(sprites.Length > 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
        
    }

}
