using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HpComp : HpComp
{
    [Header("Enemy_HpComp")]
    [SerializeField] float hitDetection = 50.0f;
    Enemy origin;
    protected override void Awake()
    {
        base.Awake();
        origin = GetComponent<Enemy>();
    }
    public override DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        origin.AddDetection(hitDetection);
        return base.GetDamage(damage);
    }
}