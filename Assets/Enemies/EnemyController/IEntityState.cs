using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityState
{
    void EnterState();

    void ExitState();

    void FrameUpdate();

    void PhysicsUpdate();
}
