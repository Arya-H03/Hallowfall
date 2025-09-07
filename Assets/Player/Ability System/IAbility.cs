using UnityEngine;

public interface IAbility
{
    public PlayerAbilitySO AbilitySO { get; set; }
    public void InjectReferences(PlayerController controller, PlayerAbilitySO abilitySO);
    public void Init();
    public void Perform();
}

public interface  IAbilityUpgrade
{
    //Apply upgrade logic to the source ability
    void ApplyUpgradeLogicTo(IAbility ability);
}

public interface IUpgradeableAbility
{
    //When an ability calls one for it's upgrades to apple it's logic to the source.
    void ApplyUpgrade(IAbilityUpgrade upgrade);
}
