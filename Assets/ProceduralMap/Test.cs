using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public Tilemap tilemap;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerGO"))
        {
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("PlayerGO"))
        {
            Debug.Log("Out");
        }
    }
}
