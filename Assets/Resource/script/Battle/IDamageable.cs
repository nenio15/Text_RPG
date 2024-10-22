using UnityEngine;

public interface IDamageable
{
    void OnDamage(float damage, Vector3 hitPos, Vector3 hitSurface);
}