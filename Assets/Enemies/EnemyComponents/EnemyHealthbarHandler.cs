using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarHandler : MonoBehaviour,IInitializeable<EnemyController>
{
    [SerializeField] private Transform healthbarFG;
    [SerializeField] private Transform worldCanvas;

    private EnemyHitHandler hitHandler;
    public void Init(EnemyController enemyController)
    {
        hitHandler = enemyController.EnemyHitHandler;
    }

    public void UpdateEnemyHealthBar()
    {
        Vector3 scale = new(hitHandler.CurrentHealth / hitHandler.MaxHealth, 1, 1);
        healthbarFG.localScale = scale;
    }

    public void DeactiveateHealthbar()
    {
        worldCanvas.gameObject.SetActive(false);
    }

    public void ActiveateHealthbar()
    {
        worldCanvas.gameObject.SetActive(true);
    }

    public void ChangeHealthbarDirection(int dir)
    {
        worldCanvas.localScale = new Vector3(dir * Mathf.Abs(worldCanvas.localScale.x), worldCanvas.localScale.y, worldCanvas.localScale.z); ;
    }
}
