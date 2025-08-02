using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerStateEnum
{
    Idle,
    Run,
    SwordAttack,
    Parry,
    Roll,
    Dash,
    Death

}
public class PlayerState : IEntityState
{
    protected PlayerStateEnum stateEnum;
    protected PlayerController playerController;
    protected PlayerConfig playerConfig;
    protected PlayerStateMachine stateMachine;
    protected PlayerSignalHub signalHub;

    public PlayerState(PlayerController playerController, PlayerStateMachine stateMachine, PlayerConfig playerConfig, PlayerStateEnum stateEnum)
    {
        this.playerController = playerController;
        this.stateMachine = stateMachine;
        this.playerConfig = playerConfig;
        this.stateEnum = stateEnum;
        this.signalHub = playerController.PlayerSignalHub;
    }

  
    public virtual void EnterState() { }
    
    public virtual void ExitState() { }
   
    public virtual void FrameUpdate() { }
   
    public virtual void PhysicsUpdate() { }
  
}
