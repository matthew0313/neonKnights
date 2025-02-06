using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Diamond Ring", menuName = "Scriptables/Items/Unstackables/Equipments/Accessories/Diamond Ring", order = 0)]
public class DiamondRing_ItemData : AccessoryItemData
{
    [Header("DiamondRing")]
    [SerializeField] float m_shieldBoost;
    [SerializeField] float m_speedBoost;
    public float shieldBoost => m_shieldBoost;
    public float speedBoost => m_speedBoost;
    public override Item Create()
    {
        return new DiamondRing_Item(this);
    }
}
public class DiamondRing_Item : AccessoryItem
{
    new public readonly DiamondRing_ItemData data;
    public DiamondRing_Item(DiamondRing_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnEquip(Player equipper)
    {
        base.OnEquip(equipper);
        equipper.movement.moveSpeed += data.speedBoost;
        equipper.hpComp.maxShield += data.shieldBoost;
    }
    public override void OnUnequip(Player equipper)
    {
        base.OnUnequip(equipper);
        equipper.movement.moveSpeed -= data.speedBoost;
        equipper.hpComp.maxShield -= data.shieldBoost;
    }

    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Shield Bonus: {data.shieldBoost}",
            kr = $"실드 증가량: {data.shieldBoost}"
        };
        yield return new()
        {
            en = $"Speed Bonus: {data.speedBoost}",
            kr = $"속도 증가량: {data.speedBoost}"
        };
    }
}