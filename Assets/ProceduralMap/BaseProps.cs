using UnityEngine;

public class BaseProps : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bounds bounds = ProceduralUtils.GetCombinedBounds(this.gameObject);

        Debug.Log("World Space Bounds Center: " + bounds.center);
        Debug.Log("World Space Size: " + bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
