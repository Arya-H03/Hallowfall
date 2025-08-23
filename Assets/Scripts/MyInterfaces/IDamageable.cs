using UnityEngine;

public interface IDamagable
{
    public int MaxHealth { get; set; }    
    public int CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    public void RestoreFullHealth(); 
    public void ApplyDamage(int amount);
    public void Die();
}
