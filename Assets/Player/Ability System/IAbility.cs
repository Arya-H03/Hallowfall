using UnityEngine;

public interface IAbility
{
    public void PassPlayerControllerRef(PlayerController controller);
    public void Initialize();
    public void Perfom();
}
