using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HangingPoint : MonoBehaviour
{
    [SerializeField] Transform hangingPoint;
    PlayerController playerController;
    private bool isHanging = false; 
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playerController)
            {
                playerController = collision.gameObject.GetComponent<PlayerController>();
            }

            else if (playerController.CanHang)
            {
                playerController.ChangeState(PlayerStateEnum.Hang);
                isHanging = true;
                
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHanging = false;
        }
    }

    private void Update()
    {
        if(isHanging)
        {
            playerController.gameObject.transform.position = hangingPoint.position;
        }
    }
}
