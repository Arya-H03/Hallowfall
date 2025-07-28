using UnityEngine;

public class CTargetMarker : MonoBehaviour
{
    [SerializeField] private Material markMaterial;
    [SerializeField] private Color markColor;

    public void MarkTarget(GameObject target)
    {
        if(target.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            sr.material = markMaterial;
            sr.material.SetColor("_OutlineColor", markColor);
        }
        
    }
}
