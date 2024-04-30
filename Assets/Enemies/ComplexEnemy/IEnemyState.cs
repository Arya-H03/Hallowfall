using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{

    void OnEnterState();

    void OnExitState();

    void HandleState();
}
