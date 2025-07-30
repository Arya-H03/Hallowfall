using System;
using UnityEngine;

public class EnemySignalHub
{
    
    public Action<float,HitSfxType> OnEnemyHit;
    public Action<float> OnEnemyDamage;
    public Action<float,float> OnEnemyHealthChange;

    public Action OnEnemyDeath;
    public Action OnEnemyDeSpawn;

    public Action<int> OnEnemyTurn;
   
}
