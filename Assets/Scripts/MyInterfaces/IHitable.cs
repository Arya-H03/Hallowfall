using UnityEngine;

public struct HitInfo
{
    public int Damage;
    public HitSfxType HitSfx;
    public Vector3 AttackerPosition;
    public float KnockbackForce;
    public bool isImmuneable;

    public HitInfo(int damage, HitSfxType hitSfx, Vector3 attackerPosition, float knockbackForce, bool isImmuneable)
    {
        Damage = damage;
        HitSfx = hitSfx;
        AttackerPosition = attackerPosition;
        KnockbackForce = knockbackForce;
        this.isImmuneable = isImmuneable;
    }
}
public interface IHitable
{
    bool HandleHit(HitInfo hitInfo);
}
