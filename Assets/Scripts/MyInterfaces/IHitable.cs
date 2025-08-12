using UnityEngine;

public struct HitInfo
{
    public float Damage;
    public HitSfxType HitSfx;
    public Vector3 AttackerPosition;
    public float KnockbackForce;

    public HitInfo(float damage, HitSfxType hitSfx, Vector3 attackerPosition, float knockbackForce)
    {
        Damage = damage;
        HitSfx = hitSfx;
        AttackerPosition = attackerPosition;
        KnockbackForce = knockbackForce;
    }
}
public interface IHitable
{
    void HandleHit(HitInfo hitInfo);
}
