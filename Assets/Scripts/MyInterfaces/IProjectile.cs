using UnityEngine;

public interface IProjectile
{
    public string TargetTag { get; set; }
    public int Damage {  get; set; }
    public float Speed { get; set; }
    public float LifeTime {  get; set; }

    public Rigidbody2D RB { get; set; }

    public void SetVelocity(float newVel) { }
    public void ResetVelocity() { }
    public void Destroy(float lifeTime) { }
}
