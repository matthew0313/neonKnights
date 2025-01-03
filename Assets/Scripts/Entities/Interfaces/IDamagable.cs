using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public DamageReceivedInfo GetDamage(float damage);
}
public struct DamageReceivedInfo
{
    public float receivedDamage;
    public bool fatal;
}