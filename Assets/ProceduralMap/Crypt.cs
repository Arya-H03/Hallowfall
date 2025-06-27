using UnityEngine;

public class Crypt : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] Sprite[] cryptSprites;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();    
    }
    private void Start()
    {
        //Crypt sprite
        sr.sprite = GetRandomSprite(cryptSprites, false);
    }




    private Sprite GetRandomSprite(Sprite[] spritePool, bool canReturnNothing)
    {
        if (spritePool.Length == 0)
        {
            Debug.LogError(spritePool + " has no sprites");
            return null;
        }   
        if (canReturnNothing)
        {
            if (Random.value < 0.5)
            {
                return spritePool[Random.Range(0, spritePool.Length)];
            }
            else return null;
        }
        else
        {
            return spritePool[Random.Range(0, spritePool.Length)];
        }
       
    }
}
