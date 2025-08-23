using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarHandler : MonoBehaviour, IInitializeable<PlayerController>
{
    private PlayerController playerController;
    private Transform healthBar;
    private TextMeshProUGUI healthText;
    private PlayerSignalHub signalHub;
    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        signalHub = playerController.PlayerSignalHub;
        healthBar = UIManager.Instance.HealthBar;
        healthText = UIManager.Instance.HealthText;

        signalHub.OnPlayerHealthChange += UpdateHealthUI;
    }

    private void OnDisable()
    {
        signalHub.OnPlayerHealthChange -= UpdateHealthUI;
    }
    private void UpdateHealthUI(int maxHealth, int currentHealth, int changedAmount)
    {
        float ratio = (float)currentHealth / maxHealth;
        healthBar.localScale = new Vector3(ratio, 1, 1);

        healthText.text = currentHealth .ToString() + "/" + maxHealth.ToString();
    }
}
