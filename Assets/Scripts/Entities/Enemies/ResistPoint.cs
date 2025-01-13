using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistPoint : MonoBehaviour, IDamagable
{
    [SerializeField] HpComp resistPointOf;
    [SerializeField] float resistPointMultiplier = 0.25f;

    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        damage.amount *= resistPointMultiplier;
        damage.isResistPointHit = true;
        DamageReceivedInfo tmp = resistPointOf.GetDamage(damage);
        tmp.isResistPointHit = true;
        return tmp;
    }
}