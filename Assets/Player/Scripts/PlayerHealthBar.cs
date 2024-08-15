using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
   
    private GameObject playerGo;
    private PlayerController playerController;

    private void Awake()
    {
        playerGo = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGo.GetComponent<PlayerController>();

    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (playerController != null)
        {
            float ratio = (float)playerController.CurrentHealth / playerController.MaxHealth;
            this.transform.localScale = new Vector3(ratio, 1, 1);
        }   
    }




}
