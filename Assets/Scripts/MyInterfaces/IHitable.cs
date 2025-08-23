using UnityEngine;

public struct HitInfo
{
    public int Damage;
    public HitSfxType HitSfx;
    public Vector3 AttackerPosition;
    public float KnockbackForce;

    public HitInfo(int damage, HitSfxType hitSfx, Vector3 attackerPosition, float knockbackForce)
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
