using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void OnEnterState();

    void OnExitState();

    void HandleState();

    void SetOnInitializeVariables(PlayerController statesManagerRef);
}
