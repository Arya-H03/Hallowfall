using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : MonoBehaviour , IEnemyState
{
    protected EnemyStateEnum stateEnum;
    protected EnemyController enemyController;

    public EnemyBaseState()
    {
        
    }

    public void SetStatesController(EnemyController statesManagerRef)
    {
        this.enemyController = statesManagerRef;  
    }
    public EnemyStateEnum GetStateEnum()
    {
        return stateEnum;
    }


    public virtual void OnEnterState()
    {

    }

    public virtual void OnExitState()
    {

    }

    public virtual void HandleState()
    {

    }

    //public void ChangeState(EnemyBaseState newState)
    //{
    //    Debug.Log(enemyController.GetCurrentStateEnum().ToString() + " to " + newState.GetStateEnum().ToString());

    //    if (enemyController.GetCurrentState() != null)
    //    {
    //        enemyController.GetCurrentState().OnExitState();
    //    }

    //    enemyController.SetCurrentState(newState);
    //    enemyController.SetCurrentStateEnum(newState.GetStateEnum());
    //    enemyController.GetCurrentState().OnEnterState();
    //}
}
