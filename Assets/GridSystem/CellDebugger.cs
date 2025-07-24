using TMPro;
using UnityEngine;

public class CellDebugger : MonoBehaviour
{
    private SpriteRenderer mainSr;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI iDText;
    [SerializeField] GameObject dirArrow;

    private void Awake()
    {
        mainSr = GetComponent<SpriteRenderer>();
    }

    public void UpdateColor(Color color)
    {
        Color newColor = new Color(color.r, color.g, color.b,0.2f);
        mainSr.color = newColor;
    }

    public void UpdateCostText(int cost)
    {
        costText.text = "Cost: " + cost;
    }

    public void UpdateIDText(Vector2Int id)
    {
        iDText.text = "ID: " + id.x + ", " + id.y;
    }

    public void DisableArrow()
    {
        dirArrow.SetActive(false);
    }

    public void UpdateArrow(Quaternion newRot)
    {
        dirArrow.transform.rotation = newRot;
        dirArrow.SetActive(true);
    }

}
