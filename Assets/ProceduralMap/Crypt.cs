using UnityEngine;

public class Crypt : MonoBehaviour
{
    [SerializeField] Transform topDecorHolder;
    [SerializeField] Sprite [] topDecorSprites;

    private void OnValidate()
    {
        MyUtils.ValidateFields(this, topDecorHolder, nameof(topDecorHolder));
        MyUtils.ValidateFields(this, topDecorSprites, nameof(topDecorSprites));
    }

    private void Awake()
    {
        foreach (Transform child in topDecorHolder)
        {
            child.GetComponent<SpriteRenderer>().sprite = MyUtils.GetRandomRef(topDecorSprites, 0.25f);
        }
    }
}
