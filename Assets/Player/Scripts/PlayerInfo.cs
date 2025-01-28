using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
  
public class PlayerInfo
{
    private int maxHealth = 100;
    private int currentHealth = 0;

    private int currentAtonement = 0;
    private int atonementLvl = 0;
    private int atonementToLevel = 3;

   
   
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentAtonement { get => currentAtonement; set => currentAtonement = value; }
    public int AtonementLvl { get => atonementLvl; set => atonementLvl = value; }
    public int AtonementToLevel { get => atonementToLevel; set => atonementToLevel = value; }


}
