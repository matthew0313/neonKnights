using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public DamageReceivedInfo GetDamage(DamageInfo damage);
}
public struct DamageInfo
{
    public float amount;
    public IDamageSource source;

    public bool ignoreShield;
    
    public bool isWeakPointHit, isResistPointHit;
    public bool ignorePointMultiplier;

    public bool dontPlayMotion;

    public static implicit operator float(DamageInfo info)
    {
        return info.amount;
    }
    public static implicit operator DamageInfo(float other)
    {
        return new DamageInfo()
        {
            amount = other
        };
    }
}
public struct DamageReceivedInfo
{
    public float receivedDamage, receivedShieldDamage;
    public float receivedDamageTotal => receivedDamage + receivedShieldDamage;

    public bool fatal;

    public bool isWeakPointHit, isResistPointHit;

    public bool dontPlayMotion;
}