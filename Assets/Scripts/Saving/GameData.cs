using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int skullCount = 0;
    public int [] skillTreeNodes = new int[12];

    public GameData(int skullCount)
    {
        this.skullCount = skullCount;
    }

    public GameData() { }
}
