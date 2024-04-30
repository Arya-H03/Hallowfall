using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class EnemyBaseState : MonoBehaviour , IEnemyState
{
    protected EnemyStateEnum stateEnum;
    protected EnemyStatesManager statesManager;

    public EnemyBaseState()
    {
        
    }

    public void SetStatesManager(EnemyStatesManager statesManagerRef)
    {
        this.statesManager = statesManagerRef;  
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
    //    Debug.Log(statesManager.GetCurrentStateEnum().ToString() + " to " + newState.GetStateEnum().ToString());

    //    if (statesManager.GetCurrentState() != null)
    //    {
    //        statesManager.GetCurrentState().OnExitState();
    //    }

    //    statesManager.SetCurrentState(newState);
    //    statesManager.SetCurrentStateEnum(newState.GetStateEnum());
    //    statesManager.GetCurrentState().OnEnterState();
    //}
}
