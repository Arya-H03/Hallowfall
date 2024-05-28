using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] GameObject [] healthShields;

    private int currenthealthShieldsIndex = 0;
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
     
    }

    private void OnEnable()
    {
        Player.LosingHealthShieldEvent += OnLifeLost;
        Player.PlayerRespawnEvent += ResetHealthShields;
    }
 
    private void OnLifeLost()
    {
        if (currenthealthShieldsIndex < 3)
        {
            healthShields[currenthealthShieldsIndex].GetComponent<HealthShield>().PlayBreakAnimation();
            currenthealthShieldsIndex++;
        }              
    }

    private void ResetHealthShields()
    {
        foreach (var healthShield in healthShields)
        {
            healthShield.GetComponent<HealthShield>().ShowHealthShield(); 
        }
    }

    

 
}
