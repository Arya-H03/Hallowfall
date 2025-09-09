using System;
using UnityEngine;

public class PlayerHealthPotionHandler : MonoBehaviour
{
    private static PlayerHealthPotionHandler instance;

    public static PlayerHealthPotionHandler Instance
    {
        get
        {
            return instance;
        }
    }

    public Action <float> OnPotionChargeRestored;
    public Action OnHealthPotionUsed;

    private Transform potionBar;
    private PlayerController playerController;

    private float maxCharges = 100;
    private float currentCharges;

    private int healAmountPerUse = 25;
    private float healCostPerUse = 40;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
            return;
        } 
        instance = this;

        currentCharges = maxCharges;
    }

    private void OnEnable()
    {
        OnPotionChargeRestored += RestoreCharges;
        OnHealthPotionUsed += UseHealthPotion;
    }
    private void OnDisable()
    {
        OnPotionChargeRestored -= RestoreCharges;
        OnHealthPotionUsed -= UseHealthPotion;
    }
    private void Start()
    {
        playerController = GameManager.Instance.PlayerController;
        potionBar = UIManager.Instance.PotionBar;

        UpdatePotionBar();
    }

    private void UseHealthPotion()
    {
        if (currentCharges < healCostPerUse) return;

        currentCharges -= healCostPerUse;
        UpdatePotionBar();

        playerController.PlayerSignalHub.OnRestoreHealth?.Invoke(healAmountPerUse);

    }

    private void RestoreCharges(float amount)
    {
        currentCharges += amount;
        if(currentCharges > maxCharges) currentCharges = maxCharges;
        UpdatePotionBar();
    }

    private void UpdatePotionBar()
    {
        float ratio = currentCharges / maxCharges;
        potionBar.transform.localScale = new Vector3(ratio,1,1);
    }
}
