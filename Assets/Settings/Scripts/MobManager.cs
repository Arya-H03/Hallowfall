using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    private static MobManager instance;

    public static MobManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MobManger");
                go.AddComponent<MobManager>();
            }

            return instance;
        }
    }


    private List<EnemyController> listOfEnemyControllers;

    private void Awake()
    {

        if(instance &&  instance!= this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;    

        EnemyController[] enemyControllers = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        listOfEnemyControllers = new List<EnemyController>(enemyControllers);


    }


    public void ResetPlayerForAllEnemies()
    {
        foreach(EnemyController enemyController in listOfEnemyControllers)
        {
            enemyController.ResetPlayer();
        }
    }
    public void ResetLookingForPlayersForAllEnemies()
    {
        foreach(EnemyController enemyController in listOfEnemyControllers)
        {
            enemyController.EnableLookingForPlayer();
        }
    }
}
