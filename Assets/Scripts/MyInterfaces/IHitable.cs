using UnityEngine;

public struct HitInfo
{
    public int damage;
    public bool canBeImmune;
    public KnockbackInfo knockbackInfo;
    public bool canFlashOnHit;
    public bool canPlayVFXOnHit;
    public bool canPlaySFXOnHit;
    public bool canPlayAnimOnHit;


    public HitInfo(int damage, bool canBeImmune, KnockbackInfo knockbackInfo,
               bool canFlashOnHit = false, bool canPlayVFXOnHit = false,
               bool canPlaySFXOnHit = false, bool canPlayAnimOnHit = false)
    {
        this.damage = damage;
        this.knockbackInfo = knockbackInfo;
        this.canBeImmune = canBeImmune;
        this.canFlashOnHit = canFlashOnHit;
        this.canPlayVFXOnHit = canPlayVFXOnHit;
        this.canPlaySFXOnHit = canPlaySFXOnHit;
        this.canPlayAnimOnHit = canPlayAnimOnHit;
    }

}

public struct KnockbackInfo
{
    public bool canKnockback;
    public Vector3 forceSourcePosition;
    public float knockbackForce;

    public KnockbackInfo(bool canKnockback = false, Vector3? forceSourcePosition = null, float knockbackForce = 0)
    {
        this.canKnockback = canKnockback;
        this.forceSourcePosition = forceSourcePosition ?? Vector3.zero;
        this.knockbackForce = knockbackForce;
    }
}
public interface IHitable
{
    bool HandleHit(HitInfo hitInfo);
}
