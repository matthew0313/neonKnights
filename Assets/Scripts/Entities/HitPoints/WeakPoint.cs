using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : HitPoint, IDamagable, IHealable, IAlimentable
{
    [Header("WeakPoint")]
    [SerializeField] HpComp weakPointOf;
    [SerializeField] float weakPointMultiplier = 3.0f;

    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        damage.amount *= weakPointMultiplier;
        damage.isWeakPointHit = true;
        return weakPointOf.GetDamage(damage);
    }
    public void GetHeal(float amount)
    {
        weakPointOf.GetHeal(amount);
    }
    public void ApplyAliment(StatusAliment aliment)
    {
        weakPointOf.ApplyAliment(aliment);
    }
}