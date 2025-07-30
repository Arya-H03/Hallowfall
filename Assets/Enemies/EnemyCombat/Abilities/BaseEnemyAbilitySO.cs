using UnityEngine;

public abstract class BaseEnemyAbilitySO : ScriptableObject, IEnemyAbility
{
    public float cooldown;
    public float range;
    public string animCondition;
    public AudioClip[] abilitySFX;
    public abstract void ExecuteAbility(EnemyController enemy);
}
