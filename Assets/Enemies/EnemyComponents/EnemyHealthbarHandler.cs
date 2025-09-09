using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarHandler : MonoBehaviour,IInitializeable<EnemyController>
{
    [SerializeField] private Transform healthbarFG;
    [SerializeField] private Transform worldCanvas;

    EnemySignalHub signalHub;
    public void Init(EnemyController enemyController)
    {
        signalHub = enemyController.SignalHub;

        signalHub.OnEnemyHealthChange += UpdateEnemyHealthBar;
        signalHub.OnDeActivateHealthbar += DeactiveateHealthbar;
        signalHub.OnActivateHealthbar += ActivateHealthbar;

        signalHub.OnEnemyTurn += ChangeHealthbarDirection;
    }

    //private void OnDisable()
    //{
    //    if (signalHub == null) return;
    //    signalHub.OnEnemyHealthChange -= UpdateEnemyHealthBar;
    //    signalHub.OnDeActivateHealthbar -= DeactiveateHealthbar;
    //    signalHub.OnActivateHealthbar -= ActivateHealthbar;
    //}

    public void UpdateEnemyHealthBar(float maxHealth, float currentHealth)
    {
        Vector3 scale = new(currentHealth / maxHealth, 1, 1);
        healthbarFG.localScale = scale;
    }

    public void DeactiveateHealthbar()
    {
        worldCanvas.gameObject.SetActive(false);
    }

    public void ActivateHealthbar()
    {
        worldCanvas.gameObject.SetActive(true);
    }

    public void ChangeHealthbarDirection(int dir)
    {
        worldCanvas.localScale = new Vector3(dir * Mathf.Abs(worldCanvas.localScale.x), worldCanvas.localScale.y, worldCanvas.localScale.z); ;
    }
}
