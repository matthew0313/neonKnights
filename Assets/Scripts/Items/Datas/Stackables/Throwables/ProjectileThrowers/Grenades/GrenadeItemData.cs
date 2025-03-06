using System;
using System.Collections;
using System.Collections.Generic;
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
    protected override Action<GameObject> GetOnHit()
    {
        Player wielder = base.wielder;
        return (GameObject hit) =>
        {
            if (hit.TryGetComponent(out IDamagable tmp)) tmp.GetDamage(new()
            {
                amount = data.damage * wielder.statistics.damageMultiplier,
                source = wielder
            });
        };
    }
    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Damage: {data.damage}",
            kr = $"대미지: {data.damage}"
        };
        foreach (var i in base.GetStats()) yield return i;
    }
}