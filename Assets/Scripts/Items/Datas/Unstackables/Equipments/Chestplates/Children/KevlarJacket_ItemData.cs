using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Kevlar Jacket", menuName = "Scriptables/Items/Unstackables/Equipments/Chestplates/KevlarJacket", order = 0)]
public class KevlarJacket_ItemData : ChestplateItemData
{
    [Header("Kevlar Jacket")]
    [SerializeField] float m_healthBoost;
    public float healthBoost => m_healthBoost;
    public override Item Create()
    {
        return new KevlarJacket_Item(this);
    }
}
public class KevlarJacket_Item : ChestplateItem
{
    new public readonly KevlarJacket_ItemData data;
    public KevlarJacket_Item(KevlarJacket_ItemData data) : base(data)
    {
        this.data = data;
    }
    public override void OnEquip(Player equipper)
    {
        base.OnEquip(equipper);
        equipper.hpComp.bonusHp += data.healthBoost;
    }
    public override void OnUnequip(Player equipper)
    {
        base.OnUnequip(equipper);
        equipper.hpComp.bonusHp -= data.healthBoost;
    }
    public override IEnumerable<LangText> GetStats()
    {
        yield return new()
        {
            en = $"Health Boost: {data.healthBoost}",
            kr = $"체력 증가량: {data.healthBoost}"
        };
    }
}