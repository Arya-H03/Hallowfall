using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Image foreGround;
    private Player player;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

   

    public void ChangeHealthBar()
    {
        if (player)
        {
            float ratio = 1-((float)player.currentHealth / player.maxHealth);
            foreGround.transform.localScale = new Vector3(ratio, 1, 1);
        }
    }
}
