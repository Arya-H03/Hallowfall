using UnityEngine;

public interface IDamagable
{
    public float MaxHealth { get; set; }    
    public float CurrentHealth { get; set; }
    public float DamageModifier { get; set; }

    public void RestoreHealth(); 
    public void ApplyDamage(float amount);
    public void Die();
}
