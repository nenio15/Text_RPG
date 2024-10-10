using System.Numerics;

internal interface IDamageable
{
    void OnDamage(float damage, Vector2 hitPos, Vector2 hitSurface);
}