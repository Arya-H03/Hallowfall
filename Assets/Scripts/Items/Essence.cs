using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : BaseItem
{
    public override void OnItemPickUp()
    {
        LevelupManager.Instance.OnEssencePickUp();
        Destroy(this.gameObject);

    }
}
