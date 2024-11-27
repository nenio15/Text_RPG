using UnityEngine;

public interface IEntityEffect
{
    void OnDamage(float damage, Vector3 hitPos, Vector3 hitSurface);

    void OnBuff(string name, float value, string state);
    void OnStat(string name, float value, string state);
    //void OnNumericalAdjust(string name, float value, string state);
}