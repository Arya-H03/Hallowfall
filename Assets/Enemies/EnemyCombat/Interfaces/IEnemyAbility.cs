using System.Collections;
using UnityEngine;

public interface IEnemyAbility
{
    public void ExecuteAbility(EnemyController enemy);
    public void EndAbility(EnemyController enemy);

    public void ActionOnAnimFrame(EnemyController enemy);

}
