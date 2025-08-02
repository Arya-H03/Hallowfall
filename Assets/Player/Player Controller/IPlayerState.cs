using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void InitState(PlayerConfig config);
    void OnEnterState();

    void OnExitState();

    void HandleState();

    void SetOnInitializeVariables(PlayerController statesManagerRef);
}
