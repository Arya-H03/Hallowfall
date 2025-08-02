using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerStateMachine
{
    private PlayerController playerController;

    private PlayerIdleState playerIdleState;
    private PlayerRunState playerRunState;
    private PlayerSwordAttackState playerSwordAttackState;
    private PlayerParryState playerParryState;
    private PlayerRollState playerRollState;
    private PlayerDeathState playerDeathState;
    private PlayerDashState playerDashState;

    private PlayerStateEnum currentStateEnum;
    private PlayerState currentState;

    public PlayerStateEnum CurrentStateEnum { get => currentStateEnum;}
    public PlayerState CurrentState { get => currentState; }
    public PlayerIdleState PlayerIdleState { get => playerIdleState; }
    public PlayerRunState PlayerRunState { get => playerRunState; }
    public PlayerSwordAttackState PlayerSwordAttackState { get => playerSwordAttackState; }
    public PlayerParryState PlayerParryState { get => playerParryState; }
    public PlayerRollState PlayerRollState { get => playerRollState; }
    public PlayerDeathState PlayerDeathState { get => playerDeathState; }
    public PlayerDashState PlayerDashState { get => playerDashState; }

    public PlayerStateMachine(PlayerController playerController)
    {
        this.playerController = playerController;
        playerController.PlayerSignalHub.OnChangeState += ChangeState;
    }

    public void InitAllStates()
    {
        playerIdleState = new PlayerIdleState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Idle);
        playerRunState = new PlayerRunState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Run);
        playerSwordAttackState = new PlayerSwordAttackState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.SwordAttack);
        playerParryState = new PlayerParryState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Parry);
        playerRollState = new PlayerRollState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Roll);
        playerDeathState = new PlayerDeathState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Death);
        playerDashState = new PlayerDashState(playerController, this, playerController.PlayerConfig, PlayerStateEnum.Dash);

        currentState = playerIdleState;
        currentStateEnum = PlayerStateEnum.Idle;
    }
    private void ChangeState(PlayerStateEnum stateEnum)
    {
        if (currentStateEnum == stateEnum) return;

        currentState?.ExitState();

        currentStateEnum = stateEnum;

        currentState = stateEnum switch
        {
            PlayerStateEnum.Idle => playerIdleState,
            PlayerStateEnum.Run => (!playerController.IsAttacking && !playerController.IsParrying) ? playerRunState : CurrentState,
            PlayerStateEnum.SwordAttack => playerSwordAttackState,
            PlayerStateEnum.Parry => playerParryState,
            PlayerStateEnum.Roll => playerRollState,
            PlayerStateEnum.Dash => playerDashState,
            PlayerStateEnum.Death => playerDeathState,
            _ => playerIdleState
        };

        currentState?.EnterState();
    }
}
