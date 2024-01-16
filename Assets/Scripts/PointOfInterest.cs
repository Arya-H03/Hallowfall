using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject intercatIcon;

    GameObject Icon;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SpawnInteractIcon();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnDestroy();
        }
    }

    private void SpawnInteractIcon()
    {
        Icon = Instantiate(intercatIcon, Vector3.zero, Quaternion.identity);
        Icon.transform.SetParent(canvas.transform, false);
        Icon.transform.position = this.transform.position + new Vector3(0.25f, 0.4f, 0);
    }

    private void OnDestroy()
    {
        Destroy(Icon);
    }
}
