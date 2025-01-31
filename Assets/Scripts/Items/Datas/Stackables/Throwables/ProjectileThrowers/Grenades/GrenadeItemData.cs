using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
public abstract class GrenadeItemData : ProjectileThrowerItemData
{
    [Header("Grenade")]
    [SerializeField] float m_damage;
    public float damage => m_damage;
}
public abstract class GrenadeItem : ProjectileThrowerItem
{
    new public readonly GrenadeItemData data;
    public GrenadeItem(GrenadeItemData data) : base(data)
    {
        this.data = data;
    }
    protected override void OnHit(GameObject hit)
    {
        if (hit.TryGetComponent(out IDamagable tmp)) tmp.GetDamage(data.damage);
    }
}