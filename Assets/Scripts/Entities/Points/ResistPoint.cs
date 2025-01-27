using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistPoint : Point, IDamagable, IHealable, IAlimentable
{
    [Header("ResistPoint")]
    [SerializeField] HpComp resistPointOf;
    [SerializeField] float resistPointMultiplier = 0.25f;

    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        damage.amount *= resistPointMultiplier;
        damage.isResistPointHit = true;
        return resistPointOf.GetDamage(damage);
    }
    public void GetHeal(float amount)
    {
        resistPointOf.GetHeal(amount);
    }
    public void ApplyAliment(StatusAliment aliment)
    {
        resistPointOf.ApplyAliment(aliment);
    }
}