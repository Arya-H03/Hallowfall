using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Out");
    }
}
