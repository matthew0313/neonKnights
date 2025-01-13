using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour, IDamagable
{
    [SerializeField] HpComp weakPointOf;
    [SerializeField] float weakPointMultiplier = 3.0f;

    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        damage.amount *= weakPointMultiplier;
        damage.isWeakPointHit = true;
        DamageReceivedInfo tmp = weakPointOf.GetDamage(damage);
        tmp.isWeakPointHit = true;
        return tmp;
    }
}