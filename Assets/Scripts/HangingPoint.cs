using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingPoint : MonoBehaviour
{
    [SerializeField] Transform hangingPoint;
    private bool isHanging = false;
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isHanging == false)
        {
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerController>().ChangeState(PlayerStateEnum.Hang);
            player.transform.position = hangingPoint.position;
            isHanging = true;


        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isHanging == true)
        {
            StartCoroutine(test());

        }
    }

    private IEnumerator test()
    {
        yield return new WaitForSeconds(1);
        isHanging = false;


    }

}
