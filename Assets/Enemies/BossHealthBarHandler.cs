using TMPro;
using UnityEngine;

public class BossHealthBarHandler : MonoBehaviour
{
    private static BossHealthBarHandler instance;

    public static BossHealthBarHandler Instance
    {
        get { return instance; }
    }

    [SerializeField] Transform foreground;
    [SerializeField] Transform background;
    [SerializeField] Transform frame;
    [SerializeField] TextMeshProUGUI bossNameTex;

    private EnemyController bossController;

    private void Awake()
    {
        if (instance != null && instance !=this)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public void SetupHealthbar(EnemyController bossController)
    {
        bossController.SignalHub.OnInitWithEnemyController += EnableHealthbar;
    }
    private void EnableHealthbar(EnemyController bossController)
    {

        this.bossController = bossController;
        bossController.SignalHub.OnEnemyHealthChange += UpdateHealthbar;
        bossController.SignalHub.OnEnemyDeathEnd += DisableHealthbar;

        UpdateHealthbar(bossController.EnemyHitHandler.MaxHealth, bossController.EnemyHitHandler.CurrentHealth);

        foreground.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        frame.gameObject.SetActive(true);
        bossNameTex.gameObject.SetActive(true);
    }

    private void DisableHealthbar()
    {
        bossController.SignalHub.OnEnemyHealthChange -= UpdateHealthbar;
        bossController.SignalHub.OnEnemyDeathEnd -= DisableHealthbar;
        bossController = null;

        foreground.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        frame.gameObject.SetActive(false);
        bossNameTex.gameObject.SetActive(false);
    }

    private void UpdateHealthbar(float maxHealth, float currentHealth)
    {
        float ratio = currentHealth / maxHealth;
        foreground.transform.localScale = new Vector3(ratio, 1, 1);
    }
}
