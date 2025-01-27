using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : Point, IDamagable, IHealable, IAlimentable
{
    [Header("ResistPoint")]
    [SerializeField] HpComp hitPointOf;

    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        return hitPointOf.GetDamage(damage);
    }
    public void GetHeal(float amount)
    {
        hitPointOf.GetHeal(amount);
    }
    public void ApplyAliment(StatusAliment aliment)
    {
        hitPointOf.ApplyAliment(aliment);
    }
}