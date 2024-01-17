using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    //[SerializeField] Image foreGround;
    private Image [] healthImages;

    //private Image currentHealthImage;
    private int currentImageIndex = 0;
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        healthImages = GetComponentsInChildren<Image>();

        // Additional null check for safety
        if (healthImages == null || healthImages.Length == 0)
        {
            Debug.LogError("No Image components found in children.");
        }
    }

    private void Update()
    {
        Debug.Log(healthImages);
    }


    public void OnLifeLost()
    {
        if (healthImages == null || currentImageIndex >= healthImages.Length)
        {
            Debug.LogError("Health images array is null or out of bounds.");
            return;
        }

        // Perform your logic with healthImages[currentImageIndex]
        healthImages[currentImageIndex].color = Color.black;
        currentImageIndex++;
    }
    //public void ChangeHealthBar()
    //{
    //    if (player)
    //    {
    //        float ratio = 1-((float)player.currentHealth / player.maxHealth);
    //        foreGround.transform.localScale = new Vector3(ratio, 1, 1);
    //    }
    //}
}
