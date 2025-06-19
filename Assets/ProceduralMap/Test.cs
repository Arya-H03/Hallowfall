using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetOccupiedCellCountFromSprite(this.gameObject, 1);
    }

    public void GetOccupiedCellCountFromSprite(GameObject obj, float cellSize)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("No SpriteRenderer found.");
            return;
        }

        Bounds bounds = sr.bounds;
        int widthInCells = Mathf.CeilToInt(bounds.size.x / cellSize);
        int heightInCells = Mathf.CeilToInt(bounds.size.y / cellSize);
        Debug.Log(widthInCells + " / " + heightInCells);
    }
}
